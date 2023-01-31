using System.Numerics;
using System;
using System.Linq;

namespace Tetris
{
    public static class Extensions
    {
        /// <summary>
        /// Checks if the piece can move down one space
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="coord"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool CanMove(this Piece piece, Coord coord, Block?[,] board)
        {
            for (int i = 0; i < piece.Width(); i++)
            {
                for (int j = 0; j < piece.Height(); j++)
                {
                    if (piece[i, j] != null)
                    {
                        if (coord.Y + j < 0)
                            continue;
                        if (coord.Y + j >= board.GetLength(1))
                            return false;
                        if (coord.X + i < 0 || coord.X + i >= board.GetLength(0))
                            return false;
                        if (board[i + coord.X, j + coord.Y] != null)
                            return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Returns a new array with the piece rotated clockwise
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece RotateCCW(this Piece piece)
        {
            Block?[,] rotated = new Block?[piece.Height(), piece.Width()];
            for (int i = 0; i < piece.Height(); i++)
            {
                for (int j = 0; j < piece.Width(); j++)
                {
                    rotated[i, j] = piece[piece.Width() - j - 1, i];
                }
            }
            return new Piece(rotated, piece.Type);
        }
        /// <summary>
        /// Returns a new array with the piece rotated counter-clockwise
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public static Piece RotateCW(this Piece piece)
        {
            Block?[,] rotated = new Block?[piece.Height(), piece.Width()];
            for (int i = 0; i < piece.Height(); i++)
            {
                for (int j = 0; j < piece.Width(); j++)
                {
                    rotated[i, j] = piece[j, piece.Height() - i - 1];
                }
            }
            return new Piece(rotated, piece.Type);
        }
        public static Piece RotateCorrectly(this Piece piece)
        {
            switch (piece.Rotation)
            {
                case Rotation.Right:
                    return piece.RotateCCW();
                case Rotation.Left:
                    return piece.RotateCW();
                case Rotation.Two:
                    return piece.RotateCW().RotateCW();
                default:
                    return piece;
            }
        }
        /// <summary>
        /// Creates a new instance of the object by doing magic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static T New<T>(this T thing)
        {
            object instance = (T)Activator.CreateInstance(typeof(T))!;
            Type type = typeof(T);
            var f = type.GetFields().ToList();
            var p = type.GetProperties().ToList();
            foreach (var field in f)
            {
                var v = field.GetValue(thing);
                field.SetValue(instance, v);
            }
            foreach (var property in p)
            {
                if (property.GetMethod == null || property.SetMethod == null)
                    continue;
                var v = property.GetValue(thing);
                property.SetValue(instance, v);
            }
            return (T)instance;
        }
        public static Block?[,] FromString(this Block?[,] matrix, string[] str)
        {
            for (int i = 0; i < Math.Min(matrix.GetLength(1), str.Length); i++)
            {
                for (int j = 0; j < Math.Min(matrix.GetLength(0), str[i].Length); j++)
                {
                    if (str[i][j] == ' ')
                        matrix[j, i] = null;
                    else
                        matrix[j, i] = new Block(str[i][j]);
                }
            }
            return matrix;
        }
        public static Microsoft.Xna.Framework.Color DivideBy(this Microsoft.Xna.Framework.Color color, int divisor)
        {
            return new Microsoft.Xna.Framework.Color(color.R / divisor, color.G / divisor, color.B / divisor);
        }
    }
}