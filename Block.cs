using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public class Block
    {
        #region Constructors
        public Block()
        {
            Color = Color.Transparent;
            IsSolid = false;
        }
        public Block(Color color)
        {
            Color = color;
            IsSolid = true;
        }
        public Block(Color color, bool isSolid)
        {
            Color = color;
            IsSolid = isSolid;
        }
        public Block(BlockType type)
        {
            Color = BlockColors[type];
            IsSolid = true;
        }
        public Block(BlockType type, bool isSolid)
        {
            Color = BlockColors[type];
            IsSolid = true;
        }
        public Block(char type)
        {
            Color = BlockTypes[type];
            IsSolid = true;
        }
        public Block(char type, bool isSolid)
        {
            Color = BlockTypes[type];
            IsSolid = type != ' ';
        }
        #endregion
        public Color Color = Color.Transparent;
        public bool IsSolid = false;

        public static Dictionary<BlockType, Color> BlockColors = new Dictionary<BlockType, Color>()
        {
            { BlockType.I, Color.Cyan },
            { BlockType.J, Color.Blue },
            { BlockType.L, Color.Orange },
            { BlockType.O, Color.Yellow },
            { BlockType.S, Color.Green },
            { BlockType.Z, Color.Red },
            { BlockType.T, Color.Purple }
        };
        public static Dictionary<char, Color> BlockTypes = new Dictionary<char, Color>()
        {
            { 'I', Color.Cyan },
            { 'J', Color.Blue },
            { 'L', Color.Orange },
            { 'O', Color.Yellow },
            { 'S', Color.Green },
            { 'Z', Color.Red },
            { 'T', Color.Purple },
            { ' ', Color.Transparent },
            { 'G', new Color( 64, 64, 64 )}
        };
    }
    public enum BlockType
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z
    }
}