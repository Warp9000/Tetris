using Microsoft.Xna.Framework;

namespace Tetris.Players
{
    public abstract class Player
    {
        public abstract PlayerReturn[] Update(GameTime gameTime, GameInfo info);
        public abstract void Initialize();
    }
    public enum PlayerReturn
    {
        Left,
        Right,
        Hold,
        RotateCW,
        RotateCCW,
        SoftDrop,
        HardDrop
    }
    public struct GameInfo
    {
        public Piece Piece;
        public Block?[,] Matrix;
        public int Score;
        public int Level;
        public int LinesCleared;
        public Piece[] NextPieces;
        public Piece? HoldPiece;
        public GameInfo(Piece piece, Block?[,] matrix, int score, int level, int linesCleared, Piece[] nextPieces, Piece? holdPiece)
        {
            Piece = piece;
            Matrix = matrix;
            Score = score;
            Level = level;
            LinesCleared = linesCleared;
            NextPieces = nextPieces;
            HoldPiece = holdPiece;
        }
    }
}