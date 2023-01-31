using System;
using System.Collections.Generic;
namespace Tetris
{
    // AAAAAAAAAAAAAAAAAAAAAAAAAAAA
    public class SRS
    {
        private static Dictionary<Tuple<Rotation, Rotation>, Coord[]> JLSTZ_KickData = new Dictionary<Tuple<Rotation, Rotation>, Coord[]>()
        {
            { Tuple.Create(Rotation.Zero, Rotation.Right),  new Coord[] { new Coord(0, 0),  new Coord(-1, 0),   new Coord(-1, -1),  new Coord(0, 2),    new Coord(-1, 2) } },
            { Tuple.Create(Rotation.Right, Rotation.Zero),  new Coord[] { new Coord(0, 0),  new Coord(1, 0),    new Coord(1, 1),    new Coord(0, -2),   new Coord(1, -2) } },
            { Tuple.Create(Rotation.Right, Rotation.Two),   new Coord[] { new Coord(0, 0),  new Coord(1, 0),    new Coord(1, 1),    new Coord(0, -2),   new Coord(1, -2) } },
            { Tuple.Create(Rotation.Two, Rotation.Right),   new Coord[] { new Coord(0, 0),  new Coord(-1, 0),   new Coord(-1, -1),  new Coord(0, 2),    new Coord(-1, 2) } },
            { Tuple.Create(Rotation.Two, Rotation.Left),    new Coord[] { new Coord(0, 0),  new Coord(1, 0),    new Coord(1, -1),   new Coord(0, 2),    new Coord(1, 2) } },
            { Tuple.Create(Rotation.Left, Rotation.Two),    new Coord[] { new Coord(0, 0),  new Coord(-1, 0),   new Coord(-1, 1),   new Coord(0, -2),   new Coord(-1, -2) } },
            { Tuple.Create(Rotation.Left, Rotation.Zero),   new Coord[] { new Coord(0, 0),  new Coord(-1, 0),   new Coord(-1, 1),   new Coord(0, -2),   new Coord(-1, -2) } },
            { Tuple.Create(Rotation.Zero, Rotation.Left),   new Coord[] { new Coord(0, 0),  new Coord(1, 0),    new Coord(1, -1),   new Coord(0, 2),    new Coord(1, 2) } }
        };

        private static Dictionary<Tuple<Rotation, Rotation>, Coord[]> I_KickData = new Dictionary<Tuple<Rotation, Rotation>, Coord[]>()
        {
            { Tuple.Create(Rotation.Zero, Rotation.Right),  new Coord[] { new Coord(0, 0),  new Coord(-2, 0),   new Coord(1, 0),    new Coord(-2, 1),  new Coord(1, -2) } },
            { Tuple.Create(Rotation.Right, Rotation.Zero),  new Coord[] { new Coord(0, 0),  new Coord(2, 0),    new Coord(-1, 0),   new Coord(2, -1),    new Coord(-1, 2) } },
            { Tuple.Create(Rotation.Right, Rotation.Two),   new Coord[] { new Coord(0, 0),  new Coord(-1, 0),   new Coord(2, 0),    new Coord(-1, -2),   new Coord(2, 1) } },
            { Tuple.Create(Rotation.Two, Rotation.Right),   new Coord[] { new Coord(0, 0),  new Coord(1, 0),    new Coord(-2, 0),   new Coord(1, 2),   new Coord(-2, -1) } },
            { Tuple.Create(Rotation.Two, Rotation.Left),    new Coord[] { new Coord(0, 0),  new Coord(2, 0),    new Coord(-1, 0),   new Coord(2, -1),    new Coord(-1, 2) } },
            { Tuple.Create(Rotation.Left, Rotation.Two),    new Coord[] { new Coord(0, 0),  new Coord(-2, 0),   new Coord(1, 0),    new Coord(-2, 1),  new Coord(1, -2) } },
            { Tuple.Create(Rotation.Left, Rotation.Zero),   new Coord[] { new Coord(0, 0),  new Coord(1, 0),    new Coord(-2, 0),   new Coord(1, 2),   new Coord(-2, -1) } },
            { Tuple.Create(Rotation.Zero, Rotation.Left),   new Coord[] { new Coord(0, 0),  new Coord(-1, 0),   new Coord(2, 0),    new Coord(-1, -2),   new Coord(2, 1) } }
        };
        public static Coord[] GetKickData(BlockType type, Rotation from, Rotation to)
        {
            if (type == BlockType.I)
                return I_KickData[Tuple.Create(from, to)];
            else
                return JLSTZ_KickData[Tuple.Create(from, to)];
        }
        public static bool RotCW(ref Piece currentPiece, ref Coord pos, Block?[,] matrix)
        {
            Rotation from = currentPiece.Rotation;
            var newPiece = currentPiece.RotateCW();
            Rotation to = (Rotation)(((int)currentPiece.Rotation + 1) % 4);
            newPiece.Rotation = to;
            Coord[] kickData = GetKickData(currentPiece.Type, from, to);
            for (int i = 0; i < kickData.Length; i++)
            {
                Coord newPos = pos + kickData[i];
                if (newPiece.CanMove(newPos, matrix))
                {
                    pos = newPos;
                    currentPiece = newPiece;
                    return true;
                }
            }
            return false;
        }
        public static bool RotCCW(ref Piece currentPiece, ref Coord pos, Block?[,] matrix)
        {
            Rotation from = currentPiece.Rotation;
            var newPiece = currentPiece.RotateCCW();
            Rotation to = (Rotation)(((int)currentPiece.Rotation + 3) % 4);
            newPiece.Rotation = to;
            Coord[] kickData = GetKickData(currentPiece.Type, from, to);
            for (int i = 0; i < kickData.Length; i++)
            {
                Coord newPos = pos + kickData[i];
                if (newPiece.CanMove(newPos, matrix))
                {
                    pos = newPos;
                    currentPiece = newPiece;
                    return true;
                }
            }
            return false;
        }
        private static Dictionary<Rotation, Coord[]> JLSTZ_OffsetData = new Dictionary<Rotation, Coord[]>()
        {
            { Rotation.Zero, new Coord[] { new Coord(0, 0), new Coord(0, 0), new Coord(0, 0), new Coord(0, 0), new Coord(0, 0) } },
            { Rotation.Right, new Coord[] { new Coord(0, 0), new Coord(1, 0), new Coord(1, -1), new Coord(0, 2), new Coord(1, 2) } },
            { Rotation.Two, new Coord[] { new Coord(0, 0), new Coord(0, 0), new Coord(0, 0), new Coord(0, 0), new Coord(0, 0) } },
            { Rotation.Left, new Coord[] { new Coord(0, 0), new Coord(-1, 0), new Coord(-1, -1), new Coord(0, 2), new Coord(-1, 2) } }
        };
        private static Dictionary<Rotation, Coord[]> I_OffsetData = new Dictionary<Rotation, Coord[]>()
        {
            { Rotation.Zero, new Coord[] { new Coord(0, 0), new Coord(-1, 0), new Coord(2, 0), new Coord(-1, 0), new Coord(2, 0) } },
            { Rotation.Right, new Coord[] { new Coord(-1, 0), new Coord(0, 0), new Coord(0, 0), new Coord(0, 1), new Coord(0, -2) } },
            { Rotation.Two, new Coord[] { new Coord(-1, 1), new Coord(1, 1), new Coord(-2, 1), new Coord(1, 0), new Coord(-2, 0) } },
            { Rotation.Left, new Coord[] { new Coord(0, 1), new Coord(0, 1), new Coord(0, 1), new Coord(0, -1), new Coord(0, 2) } }
        };

        public static Coord[] GetOffsetData(BlockType type, Rotation rotation)
        {
            if (type == BlockType.I)
                return I_OffsetData[rotation];
            else
                return JLSTZ_OffsetData[rotation];
        }

        //         An example of deriving kick translations from the offsets:

        // The offsets for J, rotation state 0 are: ( 0, 0), ( 0, 0), ( 0, 0), ( 0, 0), ( 0, 0).
        // The offsets for J, rotation state R are: ( 0, 0), (+1, 0), (+1,-1), ( 0,+2), (+1,+2).
        // ( 0, 0) - ( 0, 0) = ( 0, 0),
        // ( 0, 0) - (+1, 0) = (-1, 0),
        // ( 0, 0) - (+1,-1) = (-1,+1),
        // ( 0, 0) - ( 0,+2) = ( 0,-2),
        // ( 0, 0) - (+1,+2) = (-1,-2).

        // Therefore, the kick translations for the J rotation 0->R, relative to "true rotation" (which is conveniently the same as "basic rotation" for the J tetromino), are: ( 0, 0), (-1, 0), (-1,+1), ( 0,-2), (-1,-2).

        // public static bool RotCW(ref Piece currentPiece, ref Coord pos, Block?[,] matrix)
        // {
        //     Rotation from = currentPiece.Rotation;
        //     var newPiece = currentPiece.RotateCW();
        //     Rotation to = (Rotation)(((int)currentPiece.Rotation + 1) % 4);
        //     newPiece.Rotation = to;
        //     Coord[] offsetData1 = GetOffsetData(currentPiece.Type, from);
        //     Coord[] offsetData2 = GetOffsetData(currentPiece.Type, to);
        //     for (int i = 0; i < offsetData1.Length; i++)
        //     {
        //         Coord newPos = pos + offsetData1[i] - offsetData2[i];
        //         if (newPiece.CanMove(newPos, matrix))
        //         {
        //             pos = newPos;
        //             currentPiece = newPiece;
        //             return true;
        //         }
        //     }
        //     return false;
        // }

        // public static bool RotCCW(ref Piece currentPiece, ref Coord pos, Block?[,] matrix)
        // {
        //     Rotation from = currentPiece.Rotation;
        //     var newPiece = currentPiece.RotateCCW();
        //     Rotation to = (Rotation)(((int)currentPiece.Rotation + 3) % 4);
        //     newPiece.Rotation = to;
        //     Coord[] offsetData1 = GetOffsetData(currentPiece.Type, from);
        //     Coord[] offsetData2 = GetOffsetData(currentPiece.Type, to);
        //     for (int i = 0; i < offsetData1.Length; i++)
        //     {
        //         Coord newPos = pos + offsetData1[i] - offsetData2[i];
        //         if (newPiece.CanMove(newPos, matrix))
        //         {
        //             pos = newPos;
        //             currentPiece = newPiece;
        //             return true;
        //         }
        //     }
        //     return false;
        // }

    }
}