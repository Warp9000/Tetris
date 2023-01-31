using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tetris.Players
{
    public class TemplatePlayer : Player
    {
        public override void Initialize()
        {
            // initialize your player here
        }

        public override PlayerReturn[] Update(GameTime gameTime, GameInfo info)
        {
            List<PlayerReturn> playerReturns = new List<PlayerReturn>();

            // this is called every frame, so you can use this to check for input (or do bot stuff)

            return playerReturns.ToArray();
        }
    }
}