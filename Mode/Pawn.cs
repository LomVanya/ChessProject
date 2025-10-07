using System;

namespace MyApp
{
    public class Pawn : Figure
    {
        public Pawn(PieceColor color) : base(color) { }
        public override char ToChar() => '♟';

        public override bool IsValidMove(GameField board, int fr, int fc, int tr, int tc)
        {
            int dir = Color==PieceColor.White ? -1 : 1; // white moves up (decreasing row)
            int startRow = Color==PieceColor.White ? 6 : 1;
            // move forward
            if (fc==tc)
            {
                if (tr==fr+dir && board.Board[tr,tc]==null) return true;
                if (fr==startRow && tr==fr+2*dir && board.Board[fr+dir,fc]==null && board.Board[tr,tc]==null) return true;
            }
            // capture
            if (Math.Abs(tc-fc)==1 && tr==fr+dir)
            {
                var target = board.Board[tr,tc];
                if (target!=null && target.Color!=Color) return true;
            }
            return false;
        }
    }
}