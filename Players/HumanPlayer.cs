using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace Tetris.Players
{
    public class HumanPlayer : Player
    {
        private KeyboardState CurrentKeyboardState = Keyboard.GetState();
        private KeyboardState PreviousKeyboardState = Keyboard.GetState();

        public int ARR = 2;     // Auto Repeat Rate
        private int ARRCounter = 0;
        public int DAS = 12;    // Delayed Auto Shift
        private int DASCounter = 0;
        public int SDF = 6;     // Soft Drop Frequency
        private int SDFCounter = 0;

        Config config = new Config();

        public override void Initialize()
        {
            if (File.Exists("config.json"))
            {
                string json = File.ReadAllText("config.json");
                Config config = JsonConvert.DeserializeObject<Config>(json);
                ARR = config.ARR;
                DAS = config.DAS;
                SDF = config.SDF;
                this.config = config;
            }
            else
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText("config.json", json);
            }
        }

        public override PlayerReturn[] Update(GameTime gameTime, GameInfo info)
        {
            CurrentKeyboardState = Keyboard.GetState();
            List<PlayerReturn> playerReturns = new List<PlayerReturn>();

            bool resetDAS = true;
            if (CurrentKeyboardState.IsKeyDown(config.Left))
            {
                resetDAS = false;
                if (PreviousKeyboardState.IsKeyUp(config.Left))
                {
                    playerReturns.Add(PlayerReturn.Left);
                }
                else
                {
                    if (DASCounter >= DAS)
                    {
                        if (ARRCounter >= ARR)
                        {
                            playerReturns.Add(PlayerReturn.Left);
                            ARRCounter = 0;
                        }
                        else
                        {
                            ARRCounter++;
                        }
                    }
                    else
                    {
                        DASCounter++;
                    }
                }
            }
            if (CurrentKeyboardState.IsKeyDown(config.Right))
            {
                resetDAS = false;
                if (PreviousKeyboardState.IsKeyUp(config.Right))
                {
                    playerReturns.Add(PlayerReturn.Right);
                }
                else
                {
                    if (DASCounter >= DAS)
                    {
                        if (ARRCounter >= ARR)
                        {
                            playerReturns.Add(PlayerReturn.Right);
                            ARRCounter = 0;
                        }
                        else
                        {
                            ARRCounter++;
                        }
                    }
                    else
                    {
                        DASCounter++;
                    }
                }
            }
            if (resetDAS)
            {
                DASCounter = 0;
                ARRCounter = 0;
            }
            if (CurrentKeyboardState.IsKeyDown(config.Hold) && PreviousKeyboardState.IsKeyUp(config.Hold))
            {
                playerReturns.Add(PlayerReturn.Hold);
            }
            if (CurrentKeyboardState.IsKeyDown(config.RotateCW) && PreviousKeyboardState.IsKeyUp(config.RotateCW))
            {
                playerReturns.Add(PlayerReturn.RotateCW);
            }
            if (CurrentKeyboardState.IsKeyDown(config.RotateCCW) && PreviousKeyboardState.IsKeyUp(config.RotateCCW))
            {
                playerReturns.Add(PlayerReturn.RotateCCW);
            }
            if (CurrentKeyboardState.IsKeyDown(config.SoftDrop))
            {
                if (PreviousKeyboardState.IsKeyUp(config.SoftDrop))
                {
                    playerReturns.Add(PlayerReturn.SoftDrop);
                }
                else
                {
                    if (SDFCounter >= SDF)
                    {
                        playerReturns.Add(PlayerReturn.SoftDrop);
                        SDFCounter = 0;
                    }
                    else
                    {
                        SDFCounter++;
                    }
                }
            }
            if (CurrentKeyboardState.IsKeyDown(config.HardDrop) && PreviousKeyboardState.IsKeyUp(config.HardDrop))
            {
                playerReturns.Add(PlayerReturn.HardDrop);
            }

            PreviousKeyboardState = CurrentKeyboardState;
            return playerReturns.ToArray();
        }

        public struct Config
        {
            public Config()
            {

            }
            public Keys Left = Keys.Left;
            public Keys Right = Keys.Right;
            public Keys Hold = Keys.C;
            public Keys RotateCW = Keys.X;
            public Keys RotateCCW = Keys.Z;
            public Keys SoftDrop = Keys.Down;
            public Keys HardDrop = Keys.Space;

            public int ARR = 2;     // Auto Repeat Rate
            public int DAS = 12;    // Delayed Auto Shift
            public int SDF = 6;     // Soft Drop Frequency
        }
    }
}