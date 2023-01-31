using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tetris;

public class Tetris : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch = null!;

    // Change the player here
    // .............................................

    Players.Player Player = new Players.HumanPlayer();

    // .............................................

    private Texture2D Pixel = null!;
    private KeyboardState CurrentKeyboardState = Keyboard.GetState();
    private KeyboardState PreviousKeyboardState = Keyboard.GetState();
    private Coord ScreenCenter = new Coord(0, 0);
    private SpriteFont Font = null!;

    private int BlockSize = 32;
    private Block?[,] Matrix = new Block[10, 40];
    private Piece? CurrentPiece;
    private Piece? HeldPiece;
    private List<Piece> NextPieces = new List<Piece>();
    private bool CanHold = true;
    private Coord CurrentPiecePosition = new Coord(4, 19);
    private Random RNG = new Random();
    private Piece[] Bag7 = Pieces.AllPieces;
    private int TimeBetweenTicks = 1000;
    private bool TickNow = false;
    private Stopwatch TickStopwatch = Stopwatch.StartNew();
    private int LockDelayTime = 500;
    private bool LockNow = false;
    private Stopwatch LockDelay = new Stopwatch();
    private Stopwatch TimeSinceInput = Stopwatch.StartNew();

    private int Level = 0;
    private int Score = 0;
    private int LinesCleared = 0;
    private bool GameOver = false;

    private float SpeedCurve
    {
        get
        {
            return MathF.Pow(0.8f - ((Level - 1) * 0.007f), Level - 1);
        }
    }

    public Tetris()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
#if DEBUG
        graphics.PreferredBackBufferHeight = 720;
        graphics.PreferredBackBufferWidth = 1280;
        graphics.IsFullScreen = false;
#else
        graphics.PreferredBackBufferHeight = 1080;
        graphics.PreferredBackBufferWidth = 1920;
        graphics.IsFullScreen = true;
#endif
    }

    protected override void Initialize()
    {
        ScreenCenter = new Coord(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        BlockSize = Math.Min(GraphicsDevice.Viewport.Height / 21, GraphicsDevice.Viewport.Width / 20);
        Player.Initialize();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData(new Color[] { Color.White });
        Font = Content.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime gameTime)
    {
        CurrentKeyboardState = Keyboard.GetState();

        if (CurrentKeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        if (GameOver)
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.Enter))
            {
                GameOver = false;
                Matrix = new Block[10, 40];
                CurrentPiece = null;
                HeldPiece = null;
                NextPieces = new List<Piece>();
                CanHold = true;
                Level = 0;
                Score = 0;
                LinesCleared = 0;
            }
            base.Update(gameTime);
            return;
        }

        for (int y = 0; y < Matrix.GetLength(1); y++)
        {
            bool LineClear = true;
            for (int x = 0; x < Matrix.GetLength(0); x++)
            {
                if (Matrix[x, y] == null || !Matrix[x, y]!.IsSolid)
                {
                    LineClear = false;
                    break;
                }
            }
            if (LineClear)
            {
                for (int x = 0; x < Matrix.GetLength(0); x++)
                {
                    Matrix[x, y] = new Block();
                }
                for (int y2 = y; y2 > 0; y2--)
                {
                    for (int x = 0; x < Matrix.GetLength(0); x++)
                    {
                        Matrix[x, y2] = Matrix[x, y2 - 1];
                    }
                }
                LinesCleared++;
                Score += 100 * Level;
                if (LinesCleared % 10 == 0)
                {
                    Level++;
                }
            }
        }

        TimeBetweenTicks = (int)(1000 * SpeedCurve);

        if (CurrentPiece == null)
        {
            CurrentPiece = GetNewPiece();
            CurrentPiecePosition = GetSpawnPosition(CurrentPiece);
        }
        var input = Player.Update(gameTime, new Players.GameInfo(CurrentPiece, Matrix, Score, Level, LinesCleared, NextPieces.ToArray(), HeldPiece));
        for (int i = 0; i < input.Length; i++)
        {
            switch (input[i])
            {
                case Players.PlayerReturn.Left:
                    if (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(-1, 0), Matrix))
                    {
                        CurrentPiecePosition.X--;
                        if (LockDelay.IsRunning)
                        {
                            LockDelay.Restart();
                        }
                    }
                    break;
                case Players.PlayerReturn.Right:
                    if (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(1, 0), Matrix))
                    {
                        CurrentPiecePosition.X++;
                        if (LockDelay.IsRunning)
                        {
                            LockDelay.Restart();
                        }
                    }
                    break;
                case Players.PlayerReturn.RotateCW:
                    var lastRotation = CurrentPiece.Rotation;
                    SRS.RotCW(ref CurrentPiece, ref CurrentPiecePosition, Matrix);
                    if (lastRotation != CurrentPiece.Rotation)
                    {
                        if (LockDelay.IsRunning)
                        {
                            LockDelay.Restart();
                        }
                    }
                    break;
                case Players.PlayerReturn.RotateCCW:
                    lastRotation = CurrentPiece.Rotation;
                    SRS.RotCCW(ref CurrentPiece, ref CurrentPiecePosition, Matrix);
                    if (lastRotation != CurrentPiece.Rotation)
                    {
                        if (LockDelay.IsRunning)
                        {
                            LockDelay.Restart();
                        }
                    }
                    break;
                case Players.PlayerReturn.Hold:
                    if (CanHold)
                    {
                        if (HeldPiece == null)
                        {
                            HeldPiece = CurrentPiece.RotateCorrectly();
                            CurrentPiece = GetNewPiece();
                            CurrentPiecePosition = GetSpawnPosition(CurrentPiece);
                        }
                        else
                        {
                            var temp = HeldPiece;
                            HeldPiece = CurrentPiece.RotateCorrectly();
                            CurrentPiece = temp;
                            CurrentPiecePosition = GetSpawnPosition(CurrentPiece);
                        }
                        CanHold = false;
                    }
                    break;
                case Players.PlayerReturn.SoftDrop:
                    if (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(0, 1), Matrix))
                    {
                        CurrentPiecePosition.Y++;
                        TickStopwatch.Restart();
                    }
                    break;
                case Players.PlayerReturn.HardDrop:
                    while (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(0, 1), Matrix))
                    {
                        CurrentPiecePosition.Y++;
                    }
                    TickNow = true;
                    LockNow = true;
                    break;
            }
        }
        if (input.Count() > 0)
            TimeSinceInput.Restart();
        if (TickStopwatch.ElapsedMilliseconds > TimeBetweenTicks || TickNow)
        {
            if (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(0, 1), Matrix))
            {
                CurrentPiecePosition.Y++;
                TickStopwatch.Restart();
            }
            else
            {
                // for (int y = 0; y < CurrentPiece.Height(); y++)
                // {
                //     for (int x = 0; x < CurrentPiece.Width(); x++)
                //     {
                //         if (CurrentPiece[x, y] != null && CurrentPiecePosition.Y + y >= 0 && CurrentPiecePosition.X + x >= 0 && CurrentPiecePosition.X + x < Matrix.GetLength(0) && CurrentPiecePosition.Y + y < Matrix.GetLength(1))
                //         {
                //             Matrix[CurrentPiecePosition.X + x, CurrentPiecePosition.Y + y] = CurrentPiece[x, y]!;
                //         }
                //     }
                // }
                // CurrentPiece = GetNewPiece();
                // CurrentPiecePosition = GetSpawnPosition(CurrentPiece);
                // CanHold = true;
                TickNow = false;
                TickStopwatch.Restart();
            }
        }
        if (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(0, 1), Matrix))
        {
            LockDelay.Reset();
            LockNow = false;
        }
        else
        {
            LockDelay.Start();
        }
        if (LockDelay.ElapsedMilliseconds > LockDelayTime || LockNow)
        {
            if (CurrentPiece.CanMove(CurrentPiecePosition + new Coord(0, 1), Matrix))
            {
                LockDelay.Reset();
                LockNow = false;
            }
            else
            {
                for (int y = 0; y < CurrentPiece.Height(); y++)
                {
                    for (int x = 0; x < CurrentPiece.Width(); x++)
                    {
                        if (CurrentPiece[x, y] != null && CurrentPiecePosition.Y + y >= 0 && CurrentPiecePosition.X + x >= 0 && CurrentPiecePosition.X + x < Matrix.GetLength(0) && CurrentPiecePosition.Y + y < Matrix.GetLength(1))
                        {
                            Matrix[CurrentPiecePosition.X + x, CurrentPiecePosition.Y + y] = CurrentPiece[x, y]!;
                        }
                    }
                }
                CurrentPiece = GetNewPiece();
                CurrentPiecePosition = GetSpawnPosition(CurrentPiece);
                CanHold = true;
                LockDelay.Reset();
                LockNow = false;
            }
        }

        PreviousKeyboardState = CurrentKeyboardState;
        base.Update(gameTime);
    }

    private Piece GetNewPiece()
    {
        if (NextPieces.Count <= 5)
        {
            Bag7 = Pieces.AllPieces;
            for (int i = 0; i < 7; i++)
            {
                int index = RNG.Next(0, Bag7.Length);
                NextPieces.Add(Bag7[index]);
                Bag7 = Bag7.Where((_, i) => i != index).ToArray();
            }
        }
        var tmp = NextPieces[0];
        NextPieces.RemoveAt(0);
        bool canSpawn = true;
        var SpawnPosition = GetSpawnPosition(tmp);
        for (int y = 0; y < tmp.Height(); y++)
        {
            for (int x = 0; x < tmp.Width(); x++)
            {
                if (tmp[x, y] != null && !(Matrix[SpawnPosition.X + x, SpawnPosition.Y + y] == null || !Matrix[SpawnPosition.X + x, SpawnPosition.Y + y]!.IsSolid))
                {
                    canSpawn = false;
                }
            }
        }
        if (!canSpawn)
        {
            GameOver = true;
        }
        return tmp;
    }
    private Coord GetSpawnPosition(Piece piece)
    {
        return new Coord(Matrix.GetLength(0) / 2 - piece.Width() / 2 - piece.Width() % 2, 20);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();

        for (int x = 0; x < Matrix.GetLength(0) + 1; x++)
        {
            spriteBatch.Draw(
                Pixel,
                new Rectangle(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * x),
                    0,
                    1,
                    GraphicsDevice.Viewport.Height - BlockSize),
                new Color(64, 64, 64));
        }
        for (int y = 0; y < Matrix.GetLength(1) + 1; y++)
        {
            spriteBatch.Draw(
                Pixel,
                new Rectangle(
                    (int)(ScreenCenter.X - BlockSize * 5.5f),
                    BlockSize * y - BlockSize * 10,
                    BlockSize * 10,
                    1),
                new Color(64, 64, 64));
        }
        spriteBatch.Draw(
            Pixel,
            new Rectangle((int)(ScreenCenter.X - BlockSize * 6.5f), 0, BlockSize, BlockSize * 20),
            Color.Gray);
        spriteBatch.Draw(
            Pixel,
            new Rectangle((int)(ScreenCenter.X + BlockSize * 4.5f), 0, BlockSize, BlockSize * 20),
            Color.Gray);
        spriteBatch.Draw(
        Pixel,
        new Rectangle((int)(ScreenCenter.X - BlockSize * 6.5f), BlockSize * 20, BlockSize * 12, BlockSize),
        Color.Gray);

        for (int y = 0; y < Matrix.GetLength(1); y++)
        {
            for (int x = 0; x < Matrix.GetLength(0); x++)
            {
                if (Matrix[x, y] != null)
                {
                    spriteBatch.Draw(
                        Pixel,
                        new Rectangle(
                            (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * x),
                            BlockSize * y - BlockSize * 20,
                            BlockSize,
                            BlockSize),
                        Matrix[x, y]!.Color);
                }
            }
        }

        if (NextPieces.Count >= 1)
        {
            Coord spawnPosition = GetSpawnPosition(NextPieces[0]);
            for (int y = 0; y < NextPieces[0].Height(); y++)
            {
                for (int x = 0; x < NextPieces[0].Width(); x++)
                {
                    if (NextPieces[0][x, y] != null)
                    {
                        if (x - 1 < 0 || NextPieces[0][x - 1, y] == null)
                        {
                            spriteBatch.Draw(
                                Pixel,
                                new Rectangle(
                                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (x + spawnPosition.X)),
                                    BlockSize * (y + spawnPosition.Y) - BlockSize * 20,
                                    2,
                                    BlockSize),
                                Color.IndianRed);
                        }
                        if (x + 1 >= NextPieces[0].Width() || NextPieces[0][x + 1, y] == null)
                        {
                            spriteBatch.Draw(
                                Pixel,
                                new Rectangle(
                                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (x + 1 + spawnPosition.X)),
                                    BlockSize * (y + spawnPosition.Y) - BlockSize * 20,
                                    2,
                                    BlockSize),
                                Color.IndianRed);
                        }
                        if (y - 1 < 0 || NextPieces[0][x, y - 1] == null)
                        {
                            spriteBatch.Draw(
                                Pixel,
                                new Rectangle(
                                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (x + spawnPosition.X)),
                                    BlockSize * (y + spawnPosition.Y) - BlockSize * 20,
                                    BlockSize,
                                    2),
                                Color.IndianRed);
                        }
                        if (y + 1 >= NextPieces[0].Height() || NextPieces[0][x, y + 1] == null)
                        {
                            spriteBatch.Draw(
                                Pixel,
                                new Rectangle(
                                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (x + spawnPosition.X)),
                                    BlockSize * (y + spawnPosition.Y + 1) - BlockSize * 20,
                                    BlockSize,
                                    2),
                                Color.IndianRed);
                        }
                    }
                }
            }
        }

        if (CurrentPiece != null)
        {
            for (int y = 0; y < CurrentPiece.Height(); y++)
            {
                for (int x = 0; x < CurrentPiece.Width(); x++)
                {
                    if (CurrentPiece[x, y] != null)
                    {
                        spriteBatch.Draw(
                            Pixel,
                            new Rectangle(
                                (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (x + CurrentPiecePosition.X)),
                                BlockSize * (y + CurrentPiecePosition.Y) - BlockSize * 20,
                                BlockSize,
                                BlockSize),
                            CurrentPiece[x, y]!.Color);
                    }
                }
            }
            var GhostPiece = CurrentPiece.New();
            Coord GhostPiecePosition = CurrentPiecePosition.New();
            while (true)
            {
                GhostPiecePosition.Y++;
                if (!GhostPiece.CanMove(GhostPiecePosition, Matrix))
                {
                    GhostPiecePosition.Y--;
                    break;
                }
            }
            for (int y = 0; y < GhostPiece.Height(); y++)
            {
                for (int x = 0; x < GhostPiece.Width(); x++)
                {
                    if (GhostPiece[x, y] != null)
                    {
                        spriteBatch.Draw(
                            Pixel,
                            new Rectangle(
                                (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (x + GhostPiecePosition.X)),
                                BlockSize * (y + GhostPiecePosition.Y) - BlockSize * 20,
                                BlockSize,
                                BlockSize),
                            Color.White * 0.5f);
                    }
                }
            }
        }

        for (int i = 0; i < NextPieces.Take(5).Count(); i++)
        {
            for (int y = 0; y < NextPieces[i].Width(); y++)
            {
                for (int x = 0; x < NextPieces[i].Height(); x++)
                {
                    if (NextPieces[i][x, y] != null)
                    {
                        spriteBatch.Draw(
                            Pixel,
                            new Rectangle(
                                (int)(ScreenCenter.X + BlockSize * 6.5f + BlockSize * x),
                                BlockSize * y + BlockSize * i * 4 + BlockSize,
                                BlockSize,
                                BlockSize),
                            NextPieces[i][x, y]!.Color);
                    }
                }
            }
        }
        if (HeldPiece != null)
        {
            for (int y = 0; y < HeldPiece.Width(); y++)
            {
                for (int x = 0; x < HeldPiece.Height(); x++)
                {
                    if (HeldPiece[x, y] != null)
                    {
                        spriteBatch.Draw(
                            Pixel,
                            new Rectangle(
                                (int)(ScreenCenter.X - BlockSize * 11.5f + BlockSize * x),
                                BlockSize * y + BlockSize,
                                BlockSize,
                                BlockSize),
                            HeldPiece[x, y]!.Color);
                    }
                }
            }
        }
        spriteBatch.DrawString(
            Font,
            "Score: " + Score,
            new Vector2(ScreenCenter.X - BlockSize * 6.5f - Font.MeasureString("Score: " + Score).X, BlockSize * 5),
            Color.White);
        spriteBatch.DrawString(
            Font,
            "Lines: " + LinesCleared,
            new Vector2(ScreenCenter.X - BlockSize * 6.5f - Font.MeasureString("Lines: " + LinesCleared).X, BlockSize * 6),
            Color.White);
        spriteBatch.DrawString(
            Font,
            "Level: " + Level,
            new Vector2(ScreenCenter.X - BlockSize * 6.5f - Font.MeasureString("Level: " + Level).X, BlockSize * 7),
            Color.White);
        spriteBatch.DrawString(
            Font,
            "Next",
            new Vector2(ScreenCenter.X + BlockSize * 6.5f, BlockSize / 3),
            Color.White);
        spriteBatch.DrawString(
            Font,
            "Hold",
            new Vector2(ScreenCenter.X - BlockSize * 11.5f, BlockSize / 3),
            Color.White);
        if (GameOver)
        {
            spriteBatch.DrawString(
                Font,
                "Game Over",
                new Vector2(ScreenCenter.X - Font.MeasureString("Game Over").X / 2, ScreenCenter.Y - Font.MeasureString("Game Over").Y / 2),
                Color.White);
            spriteBatch.DrawString(
                Font,
                "Press Enter to restart",
                new Vector2(ScreenCenter.X - Font.MeasureString("Press Enter to restart").X / 2, ScreenCenter.Y + Font.MeasureString("Game Over").Y / 2),
                Color.White);
        }

#if DEBUG
        if (CurrentPiece != null)
        {
            spriteBatch.Draw(
                Pixel,
                new Rectangle(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20,
                    BlockSize * CurrentPiece.Width(),
                    1),
                Color.Red);
            spriteBatch.Draw(
                Pixel,
                new Rectangle(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20,
                    1,
                    BlockSize * CurrentPiece.Height()),
                Color.Red);
            spriteBatch.Draw(
                Pixel,
                new Rectangle(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * (CurrentPiecePosition.X + CurrentPiece.Width())),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20,
                    1,
                    BlockSize * CurrentPiece.Height()),
                Color.Red);
            spriteBatch.Draw(
                Pixel,
                new Rectangle(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * (CurrentPiecePosition.Y + CurrentPiece.Height()) - BlockSize * 20,
                    BlockSize * CurrentPiece.Width(),
                    1),
                Color.Red);

            spriteBatch.DrawString(
                Font,
                CurrentPiece.Rotation.ToString(),
                new Vector2(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20),
                Color.White);
            spriteBatch.DrawString(
                Font,
                CurrentPiecePosition.ToString(),
                new Vector2(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20 + Font.MeasureString("0").Y),
                Color.White);
            spriteBatch.DrawString(
                Font,
                TickStopwatch.ElapsedMilliseconds.ToString() + "/" + TimeBetweenTicks.ToString(),
                new Vector2(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20 + Font.MeasureString("0").Y * 2),
                Color.White);
            spriteBatch.DrawString(
                Font,
                LockDelay.ElapsedMilliseconds.ToString() + "/" + LockDelayTime.ToString(),
                new Vector2(
                    (int)(ScreenCenter.X - BlockSize * 5.5f + BlockSize * CurrentPiecePosition.X),
                    BlockSize * CurrentPiecePosition.Y - BlockSize * 20 + Font.MeasureString("0").Y * 3),
                Color.White);
        }
#endif
        spriteBatch.End();
        base.Draw(gameTime);
    }
}
