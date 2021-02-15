public partial class Map
{
    public struct MapPosition
    {
        public MapPosition(int _X, int _Y)
        {
            X = _X;
            Y = _Y;
        }

        public MapPosition Normalize()
        {
            var _X = X;
            if (_X < 0)
                _X = -1;
            else if (_X > 0)
                _X = 1;

            var _Y = Y;
            if (_Y < 0)
                _Y = -1;
            else if (_Y > 0)
                _Y = 1;

            return new MapPosition(_X, _Y);
        }

        public int X, Y;

        public override string ToString()
        {
            return "(X=" + X + "," + "Y="+ Y+ ")";
        }

        public static MapPosition operator +(MapPosition a, MapPosition b)
        => new MapPosition(a.X + b.X, a.Y + b.Y);

        public static MapPosition operator -(MapPosition a, MapPosition b)
        => new MapPosition(a.X - b.X, a.Y - b.Y);

        public static bool operator ==(MapPosition a, MapPosition b)
        => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(MapPosition a, MapPosition b)
        => a.X != b.X || a.Y != b.Y;

        public static MapPosition operator *(MapPosition a, int b)
        => new MapPosition (a.X * b, a.Y * b);

        public override bool Equals(object obj)
        {
            return obj is MapPosition position &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
