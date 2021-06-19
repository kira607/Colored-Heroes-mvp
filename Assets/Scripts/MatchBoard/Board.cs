namespace MatchBoard
{
    public class Board
    {
        private Cell[,] _board;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            _board = new Cell[width, height];
        }
    
        public ChipColor GetColorAtPoint(Point p)
        {
            if(p.x < 0 || p.x >= Width || p.y < 0 || p.y >= Height) return ChipColor.Hole;
            return _board[p.x, p.y].color;
        }

        public void SetColorAtPoint(Point point, ChipColor color)
        {
            _board[point.x, point.y].color = color;
        }

        public Cell this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                {
                    return null;
                }
                return _board[x, y];
            }
            set
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                {
                    return;
                }
                _board[x, y] = value;
            }
        }
    
        public Cell this[Point point]
        {
            get
            {
                if (point.x < 0 || point.x >= Width || point.y < 0 || point.y >= Height)
                {
                    return null;
                }
                return _board[point.x, point.y];
            }
            set
            {
                if (point.x < 0 || point.x >= Width || point.y < 0 || point.y >= Height)
                {
                    return;
                }
                _board[point.x, point.y] = value;
            }
        }
    }
}