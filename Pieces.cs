using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris
{
    public static class Pieces
    {
        public readonly static Piece I_Piece = new Piece(new Block?[4, 4]
        {
            { null, new Block(BlockType.I), null, null },
            { null, new Block(BlockType.I), null, null },
            { null, new Block(BlockType.I), null, null },
            { null, new Block(BlockType.I), null, null }
        }, BlockType.I);
        public readonly static Piece J_Piece = new Piece(new Block?[3, 3]
        {
            { new Block(BlockType.J), new Block(BlockType.J), null },
            { null, new Block(BlockType.J), null },
            { null, new Block(BlockType.J), null }
        }, BlockType.J);
        public readonly static Piece L_Piece = new Piece(new Block?[3, 3]
        {
            { null, new Block(BlockType.L), null },
            { null, new Block(BlockType.L), null },
            { new Block(BlockType.L), new Block(BlockType.L), null }
        }, BlockType.L);
        public readonly static Piece O_Piece = new Piece(new Block?[2, 2]
        {
            { new Block(BlockType.O), new Block(BlockType.O) },
            { new Block(BlockType.O), new Block(BlockType.O) }
        }, BlockType.O);
        public readonly static Piece S_Piece = new Piece(new Block?[3, 3]
        {
            { null, new Block(BlockType.S), null },
            { new Block(BlockType.S), new Block(BlockType.S), null },
            { new Block(BlockType.S), null, null }
        }, BlockType.S);
        public readonly static Piece Z_Piece = new Piece(new Block?[3, 3]
        {
            { new Block(BlockType.Z), null, null },
            { new Block(BlockType.Z), new Block(BlockType.Z), null },
            { null, new Block(BlockType.Z), null }
        }, BlockType.Z);
        public readonly static Piece T_Piece = new Piece(new Block?[3, 3]
        {
            { null, new Block(BlockType.T), null },
            { new Block(BlockType.T), new Block(BlockType.T), null },
            { null, new Block(BlockType.T), null }
        }, BlockType.T);
        public readonly static Piece[] AllPieces = new Piece[7]
        {
            I_Piece,
            J_Piece,
            L_Piece,
            O_Piece,
            S_Piece,
            T_Piece,
            Z_Piece
        };
    };
}