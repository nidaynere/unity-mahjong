using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class UserData
{
    private static UserData Instance;

    const string userDataNameOnPlayerPrefs = "userData";

    [Serializable]
    public class Map
    {
        public Map(string _Id)
        {
            Id = _Id;
            HighScore = 0;
        }

        public string Id;
        public int HighScore;
    }

    public List<Map> Maps = new List<Map>();

    public Dictionary<string, Map> IndexedMaps = new Dictionary<string, Map>();

    public static int GetHighScore(string mapName)
    {
        if (!Instance.IndexedMaps.ContainsKey(mapName))
            return 0;

        return Instance.IndexedMaps[mapName].HighScore;
    }

    public static void SetHighScore(string mapName, int score)
    {
        if (!Instance.IndexedMaps.ContainsKey(mapName))
        {
            var newMap = new Map(mapName);
            Instance.Maps.Add(newMap);
            Instance.IndexedMaps.Add(mapName, newMap);
        }

        Instance.IndexedMaps[mapName].HighScore = score;

        Save();
    }

    public static void Load()
    {
        Instance = new UserData();
        var data = PlayerPrefs.GetString(userDataNameOnPlayerPrefs, "{}");
        JsonUtility.FromJsonOverwrite(data, Instance);

        foreach (var map in Instance.Maps)
            Instance.IndexedMaps.Add(map.Id, map);
    }

    public static void Save()
    {
        PlayerPrefs.SetString(userDataNameOnPlayerPrefs, JsonUtility.ToJson(Instance));
    }
}