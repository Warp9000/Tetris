using Microsoft.Xna.Framework;
namespace Tetris
{
    public struct Coord
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Coord()
        {
            X = -1;
            Y = -1;
        }
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Coord(Coord coord)
        {
            X = coord.X;
            Y = coord.Y;
        }
        public Point ToPoint()
        {
            return new Point(X, Y);
        }
        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.X + b.X, a.Y + b.Y);
        }
        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.X - b.X, a.Y - b.Y);
        }
        public static Coord operator *(Coord a, Coord b)
        {
            return new Coord(a.X * b.X, a.Y * b.Y);
        }
        public static Coord operator /(Coord a, Coord b)
        {
            return new Coord(a.X / b.X, a.Y / b.Y);
        }
        public static Coord operator *(Coord a, int b)
        {
            return new Coord(a.X * b, a.Y * b);
        }
        public static Coord operator /(Coord a, int b)
        {
            return new Coord(a.X / b, a.Y / b);
        }
        public static bool operator ==(Coord a, Coord b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Coord a, Coord b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        public override bool Equals(object? obj)
        {
            if (obj is Coord)
            {
                Coord coord = (Coord)obj;
                return coord == this;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}