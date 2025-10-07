using System;

namespace MyApp
{
    public class Knight : Figure
    {
        public Knight(PieceColor color) : base(color) { }
        public override char ToChar() => '♞';
        public override bool IsValidMove(GameField board, int fr, int fc, int tr, int tc)
        {
            int dr = Math.Abs(fr-tr);
            int dc = Math.Abs(fc-tc);
            if ((dr==1 && dc==2) || (dr==2 && dc==1))
            {
                var dest = board.Board[tr,tc];
                if (dest==null || dest.Color!=Color) return true;
            }
            return false;
        }
    }
}