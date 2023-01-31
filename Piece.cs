namespace Tetris
{
    public class Piece
    {
        public Block?[,] Blocks { get; set; }
        public Rotation Rotation { get; set; }
        public BlockType Type { get; set; }
        public int Width() => Blocks.GetLength(0);
        public int Height() => Blocks.GetLength(1);
        public Piece(Block?[,] blocks, BlockType type)
        {
            Blocks = blocks;
            Type = type;
        }
        public Piece()
        {
            Blocks = new Block?[0, 0];
        }
        public Block? this[int x, int y] => Blocks[x, y];
    }
}