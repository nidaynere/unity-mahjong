using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform mapItemHolder;
    [SerializeField] private UIMapItem mapItemReference;

    [SerializeField] private UIAnimatedPanel failurePanel;
    [SerializeField] private UIAnimatedPanel successPanel;
    [SerializeField] private UIAnimatedPanel levelSelectionPanel;
    [SerializeField] private UIAnimatedPanel inGamePanel;

    [SerializeField] private Text successPanel_score;
    [SerializeField] private Text successPanel_highScore;
    [SerializeField] private GameObject successPanel_star;
    [SerializeField] private GameObject successPanel_highsScoreAchieved;
    [SerializeField] private GameObject successPanel_highScoreValueHolder;

    [SerializeField] private Text scoreText;

    private Dictionary<string, UIMapItem> maps = new Dictionary<string, UIMapItem>();

    // Start is called before the first frame update
    void Start ()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);

        GameEvents.OnGameStarting += () => {
            CloseLevelSelectionPanel();
            OpenInGamePanel();
        };

        GameEvents.OnScoreUpdate += (score) => { scoreText.text = score.ToString(); };
        GameEvents.OnGameFinished += (map, value, score) => {
            if (!value)
            {
                OpenFailurePanel();
            }
            else
            {
                var highScore = UserData.GetHighScore(map);

                Debug.Log(highScore + " " + score);

                var achievedNewScore = score > highScore;
                OpenSuccessPanel(score, highScore, highScore == 0, achievedNewScore);

                if (achievedNewScore)
                {
                    UserData.SetHighScore(map, score);
                }

                RefreshMaps(map);
            }

            CloseInGamePanel();
            OpenLevelSelectionPanel();
        };

        CreateMaps();
        RefreshMaps();

        OpenLevelSelectionPanel();
    }

    private void CreateMaps()
    {
        // im doing it twice :( (also at MapLoader.cs)
        var allLevels = Resources.LoadAll<TextAsset>(MapLoader.Folder);

        foreach (var level in allLevels)
        {
            var newMap = Instantiate(mapItemReference, mapItemHolder);
            newMap.Set(level.name);
            maps.Add(level.name, newMap);
        }
    }

    private void RefreshMaps(string mapName = null)
    {
        if (string.IsNullOrEmpty(mapName))
        {
            // refresh all.
            foreach (var map in maps)
            {
                bool isCompleted = UserData.GetHighScore(map.Key) > 0;
                map.Value.Refresh(isCompleted);
            }
        }
        else
        {
            // refresh only one.
            if (maps.ContainsKey(mapName))
            {
                bool isCompleted = UserData.GetHighScore(mapName) > 0;
                maps[mapName].Refresh(isCompleted);
            }
        }
    }

    private void OpenFailurePanel()
    {
        failurePanel.Open();
    }

    private void OpenSuccessPanel(int score, int highScore, bool showStars, bool achievedScore)
    {
        successPanel_highScore.text = highScore.ToString();
        successPanel_score.text = score.ToString();
        successPanel_star.SetActive(showStars);
        successPanel_highsScoreAchieved.SetActive(achievedScore);
        successPanel_highScoreValueHolder.SetActive(!showStars);

        successPanel.Open();
    }

    private void OpenLevelSelectionPanel()
    {
        levelSelectionPanel.Open();
    }

    private void CloseLevelSelectionPanel()
    {
        levelSelectionPanel.Close();
    }

    private void OpenInGamePanel()
    {
        inGamePanel.Open();
    }

    private void CloseInGamePanel()
    {
        inGamePanel.Close();
    }
}
