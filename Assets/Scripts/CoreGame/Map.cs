using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Map
{
    public struct MapMember
    {
        public MapMember(string _Id, int _X, int _Y, int _Index)
        {
            Id = _Id;
            Position = new MapPosition(_X, _Y);
            Index = _Index;
        }

        public string Id;
        public MapPosition Position;
        public int Index;

        public void UnOccupie(ref bool[][] grid)
        {
            grid[Position.Y][Position.X] = false;
        }
    }

    /// <summary>
    /// variable will be used for initialization
    /// </summary>
    private int t_Occupier;

    #region Public map variables
    public MapMember[] Members;
    public int SizeX, SizeY;
    #endregion

    public int TotalValids
    {
        get;
        private set;
    }

    #region Pathfinding

    private bool[][] grid;

    /// <summary>
    /// Returns a path from given target indexes.
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="TargetIndex"></param>
    /// <returns></returns>
    public List<MapPosition> GetPath (MapPosition startPosition, MapPosition targetPosition)
    {
        List<List<MapPosition>> found = new List<List<MapPosition>>();

        /* OOOOO    OOOOO
         * OTOOO    OOOOO
         * OOCOX    OOOOO
         * OOOXO    OOOOO
         * OOOOO    OOOOO
         * */

        Dictionary<int, MapPosition> directions = new Dictionary<int, MapPosition>();

        directions.Add(0, new MapPosition(0, 1));
        directions.Add(1, new MapPosition(0, -1));
        directions.Add(2, new MapPosition(1, 0));
        directions.Add(3, new MapPosition(-1, 0));

        MapPosition distance = targetPosition - startPosition;

        for (int i = 0; i < 4; i++)
        {
            int protection = 1000;

            int step = 0;
            while (protection > 0)
            { // go to this direction.
                protection--;
                bool isFailed = false;
                bool targetIsFound = false;

                step++;

                List<MapPosition> path = new List<MapPosition>();

                path.Add(startPosition);

                var currentPosition = startPosition + directions[i] * step;

                if (currentPosition != targetPosition)
                {
                    if (IsBlocked(currentPosition))
                    {
                        break;
                    }

                    path.Add(currentPosition);

                    int max = i < 2 ? distance.X : distance.Y;
                    int dir = Mathf.Clamp(max, -1, 1);
                    max = Mathf.Abs(max);

                    for (int s = 0; s < max; s++)
                    {
                        if (i < 2)
                            currentPosition.X += dir;
                        else currentPosition.Y += dir;

                        if (currentPosition == targetPosition)
                        {
                            targetIsFound = true;
                            break;
                        }

                        if (IsBlocked(currentPosition))
                        {
                            isFailed = true;
                            break;
                        }
                    }
                }
                else
                {
                    targetIsFound = true;
                }

                if (isFailed)
                {
                    if (protection > 0)
                        continue;
                }

                path.Add(currentPosition);
                // go to target, almost there.

                if (!targetIsFound)
                {
                    // should be at straight direction.
                    var strDistance = targetPosition - currentPosition;


                    var dir = strDistance.X != 0 ? strDistance.X : strDistance.Y;

                    var count = Mathf.Abs(dir);
                    var normal = strDistance.Normalize();

                    for (int e = 0; e < count; e++)
                    {
                        currentPosition += normal;

                        if (currentPosition == targetPosition)
                        {
                            targetIsFound = true;
                            path.Add(targetPosition);
                            break;
                        }

                        if (IsBlocked(currentPosition))
                        {
                            break;
                        }
                    }

                    if (!targetIsFound)
                    {
                        continue;
                    }
                }

                if (targetIsFound)
                {
                    found.Add(path);
                    break;
                }

            }
        }

        // no path found.
        if (found.Count == 0)
            return null;

        // find the best path.
        return found.OrderBy (x=>CornerCount (x)).ElementAt (0);
    }

    /// <summary>
    /// Is the given points are blocked?
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    private bool IsBlocked(int X, int Y)
    {
        if (Y < 0 || Y >= grid.Length)
        {
            return true;
        }

        if (X < 0 || X >= grid[Y].Length)
        {
            return true;
        }

        return grid[Y][X];
    }

    private bool IsBlocked(MapPosition position)
    {
        return IsBlocked(position.X, position.Y);
    }

    /// <summary>
    /// Returns a path from given targets.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public List<MapPosition> GetPath(MapMember from, MapMember to)
    {
        return GetPath(from.Position, to.Position);
    }
    public List<MapPosition> GetPath(int from, int to)
    {
        return GetPath(Members[from], Members[to]);
    }

    /// <summary>
    /// Returns the corner count of a non diagonal path.
    /// </summary>
    /// <param name="positions"></param>
    /// <returns></returns>
    private int CornerCount (List<MapPosition> positions)
    {
        int corners = 0;
        int lastDir = -1;
        int count = positions.Count;
        for (int i = 1; i < count; i++)
        {
            int dir = -1;
                if (positions[i].X == positions[i - 1].X)
                    dir = 1; // y is changing.
                else if (positions[i].Y == positions[i - 1].Y)
                    dir = 0; // x is changing.

            if (dir != -1)
            {
                if (lastDir != -1 && lastDir != dir)
                {
                    corners++;
                }

                lastDir = dir;
            }
        }

        return corners;
    }

    /// <summary>
    /// Check if its valid match.
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="TargetIndex"></param>
    /// <returns></returns>
    public bool IsValidMatch(int Index, int TargetIndex)
    {
        var from = Members[Index];
        var to = Members[TargetIndex];
        
        if (!from.Id.Equals(to.Id))
        {
            return false; // Id doesnt match.
        }

        var path = GetPath(from, to);
        return path != null;
    }

    public bool GetOneAvailable (out int Index, out int TargetIndex)
    {
        Index = -1;
        TargetIndex = -1;

        var list = Members.ToList();
        int memberCount = Members.Length;
        for (int i = 0; i < memberCount; i++)
        {
            Index = i;
            // siblings =>
            TargetIndex = list.FindIndex (x => !string.IsNullOrEmpty (x.Id) && x.Index != i && 
            x.Id.Equals(Members[i].Id) && IsValidMatch(i, x.Index)
            );

            if (TargetIndex != -1)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveMember(int Index)
    {
        if (string.IsNullOrEmpty(Members[Index].Id))
            return;

        Members[Index].UnOccupie(ref grid);
        Members[Index] = new MapMember();

        TotalValids--;
    }

    /// <summary>
    /// Shows all valid targets for given Index.
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public MapMember[] ShowValidTargets (int Index)
    {
        var from = Members[Index];
        var possibleIds = Members.ToList().FindAll(x => x.Id.Equals(from.Id));
        return possibleIds.FindAll(x => x.Index != Index && IsValidMatch(Index, x.Index)).ToArray ();
    }

    #endregion

    public Map (string mapName, string[] memberIds)
    {
        var loader = new MapLoader();
        grid = loader.Load(mapName, out SizeX, out SizeY);

        Members = new MapMember[SizeX * SizeY];

        // I need to do it with parser. Sorry to myself.
        List<string> validIndexes = new List<string>();

        // create a valid map.
        for (int _y = 0; _y < SizeY; _y++)
        {
            for (int _x = 0; _x < SizeX; _x++)
                if (grid[_y][_x])
                    validIndexes.Add(_x + "x" + _y);
        }

        TotalValids = validIndexes.Count;

        // Check for valid map count. It should be 2x. WTF :D
        if (TotalValids % 2 == 1)
        {
            Debug.LogWarning("Map has " + validIndexes.Count + ", which is invalid. Fixing by 1");
            validIndexes.RemoveAt(validIndexes.Count - 1);
            TotalValids--;
        }

        void occupiePoint (string Id, int Index)
        {
            var targetPoint = validIndexes.ElementAt(Index);
            var deParse = targetPoint.Split('x');
            int x = int.Parse(deParse[0]);
            int y = int.Parse(deParse[1]);

            Members[t_Occupier] = new MapMember(Id, x, y, t_Occupier);
            validIndexes.RemoveAt(Index);

            t_Occupier ++;
        }

        int membersCount = memberIds.Length;

        //Random generator;
        while (validIndexes.Count > 0)
        {
            var randPoint = UnityEngine.Random.Range(0, validIndexes.Count);

            // put a member to the first point.
            var randMember = memberIds[UnityEngine.Random.Range(0, membersCount)];

            occupiePoint(randMember, randPoint);

            // create the sibling.
            randPoint = UnityEngine.Random.Range(0, validIndexes.Count);
            occupiePoint(randMember, randPoint);
        }
    }
}
