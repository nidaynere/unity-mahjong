using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LineRenderer pathVisualizer;
    [SerializeField] private Vector2 gameMemberStep;
    [SerializeField] private CameraSetting camSetting;
    [SerializeField] private Transform mapBounds;
    [SerializeField] private Text gameLog;

    private string loadedMapName;

    private string[] imageNames;

    private Dictionary<string, Sprite> imageList = new Dictionary<string, Sprite>();

    private bool isInitialized = false;

    private Pool pool;

    private Map map;

    private int _score;
    private int score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;

            GameEvents.OnScoreUpdate?.Invoke(_score);

            if (value <= 0)
            {
                GameEvents.OnGameFinished?.Invoke(loadedMapName, false, score);
            }
        }
    }

    private Dictionary<int, GameMapMember> gameMapMembers = new Dictionary<int, GameMapMember>();

    private void Initialize ()
    {
        var allImages = Resources.LoadAll<Sprite>("TileSetFaces");
        int length = allImages.Length;
        imageNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            imageNames[i] = allImages[i].name;
            imageList.Add(imageNames[i], allImages[i]);
        }

        pool = Pool.Instance;

        GameEvents.OnGameFinished += (_, _1, _2) => { pool.Reset(); };

        isInitialized = true;
    }

    private void Load (string mapName)
    {
        score =100; // reset score.
        loadedMapName = mapName;

        if (!isInitialized)
        {
            Initialize();
        }

        GameEvents.OnUserDeselect = () =>
        {
            if (GameMapMember.Active != null)
            {
                GameMapMember.Active.SetHighlight(false);
                GameMapMember.Active.SetActiveness(false);
                GameMapMember.Active = null;
            }
        };

        GameEvents.OnUserDeselect?.Invoke();

        GameEvents.OnUserWantsHint = () => {
            // 
            if (
            map.GetOneAvailable(out int id, out int tId))
            {
                score -= 40;
                gameLog.text = "hint found!";
                gameMapMembers[id].SetHighlight(true);
                gameMapMembers[tId].SetHighlight(true);
            }
            else
            {
                gameLog.text = "no hint found. gameover.";
            }
        };

        GameMapMember.Active = null;
        pool.Reset();
        gameMapMembers.Clear();

        map = new Map(mapName, imageNames);

        int mapSizeX = map.SizeX;
        int mapSizeY = map.SizeY;

        var scale = new Vector3(mapSizeX * gameMemberStep.x, mapSizeY * -gameMemberStep.y, 1);;
        mapBounds.localScale = scale + Vector3.one;
        mapBounds.localPosition = new Vector3(scale.x /2f - 0.5f, -scale.y/ 2f + 0.5f, 10);

        // set game camera.
        var cameraTransform = Camera.main.transform;
        cameraTransform.position = camSetting.StartingPosition + new Vector3 (map.SizeX * camSetting.PositionModifierByMapSize, 
            -map.SizeY * camSetting.PositionModifierByMapSize);
        Camera.main.orthographicSize = camSetting.DefaultOrtoSize + new Vector2(map.SizeX, map.SizeY).magnitude * camSetting.OrtoSizeByMapSize;


        // visualize.
        int totalMembers = map.Members.Length;
        for (int i=0; i<totalMembers;i++)
        {
            if (string.IsNullOrEmpty(map.Members[i].Id))
            {
                // empty grid point:D
                continue;
            }

            var obj = pool.Get();
            obj.gameObject.SetActive(true);
            obj.transform.localPosition = new Vector2(map.Members[i].Position.X * gameMemberStep.x, map.Members[i].Position.Y * gameMemberStep.y);
            gameMapMembers.Add(map.Members[i].Index, obj);
            obj.Set(
                map.Members[i].Index,
                imageList[map.Members[i].Id],
                CheckMatch
                );
        }
    }

    private void CheckMatch(int from, int to)
    {
        bool isValidMatch = map.IsValidMatch(from, to);
        Debug.Log("is valid? " + isValidMatch);

        if (isValidMatch)
        {
            gameLog.text = "that's a match! nice.";

            // visualize.
            #region visualize path.
            var path = map.GetPath(from, to);

            if (path == null)
                path = new List<Map.MapPosition>();

            int pathCount = path.Count;
            Vector3[] unityPath = new Vector3[pathCount];
            for (int p = 0; p < pathCount; p++)
            {
                unityPath[p] = new Vector3(path[p].X * gameMemberStep.x, path[p].Y * gameMemberStep.y);
            }

            pathVisualizer.positionCount = pathCount;
            pathVisualizer.SetPositions(unityPath);
            #endregion

            map.RemoveMember(from);
            map.RemoveMember(to);

            gameMapMembers[from].gameObject.SetActive(false);
            gameMapMembers[to].gameObject.SetActive(false);

            gameMapMembers.Remove(from);
            gameMapMembers.Remove(to);

            score += 20;

            Debug.Log("Total valid left => " + map.TotalValids);

            if (map.TotalValids == 0)
            {
                // game end with success!!
                GameEvents.OnGameFinished?.Invoke(loadedMapName, true, score);
            }
        }
        else
        {
            gameLog.text = "wrong decision.. this is not a match.";
            score -= 10;
        }

        foreach (var member in gameMapMembers)
            member.Value.SetHighlight(false);
    }

    public static void LoadGame (string mapName)
    {
        GameEvents.OnGameStarting?.Invoke();
        var gameLoader = FindObjectOfType<GameManager>();    
        gameLoader.Load(mapName);
    }
}
