using UnityEngine;
using System.Linq;

public class MapLoader {
    public MapLoader () {

    }

    public const string Folder = "Levels/";

    public bool [][] Load (string LevelName, out int sizeX, out int sizeY) {
        var path = Folder + LevelName;
        Debug.Log("Loading map from path=>" + path);
        var file = Resources.Load<TextAsset> (path);

        var y = file.text.GetLines(true).ToArray ();

        sizeY = y.Length;
        sizeX = y[0].Length;

        bool[][] grid = new bool[sizeY][];

        for (int _y = 0; _y < sizeY; _y++)
        {
            grid[_y] = new bool[sizeX];
            
            for (int _x = 0; _x < sizeX; _x++)
            {
                grid[_y][_x] = y[_y][_x].Equals('X');
            }
        }

        return grid;
     }
}