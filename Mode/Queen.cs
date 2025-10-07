using System;

namespace MyApp
{
    public class Queen : Figure
    {
        public Queen(PieceColor color) : base(color) { }
        public override char ToChar() => '♛';
        public override bool IsValidMove(GameField board, int fr, int fc, int tr, int tc)
        {
            // rook-like
            if (fr==tr || fc==tc)
            {
                int dr = Math.Sign(tr-fr);
                int dc = Math.Sign(tc-fc);
                int r=fr+dr, c=fc+dc;
                while (r!=tr || c!=tc)
                {
                    if (board.Board[r,c]!=null) return false;
                    r+=dr; c+=dc;
                }
                var dest = board.Board[tr,tc];
                if (dest==null || dest.Color!=Color) return true;
                return false;
            }
            // bishop-like
            if (Math.Abs(fr-tr)==Math.Abs(fc-tc))
            {
                int dr = Math.Sign(tr-fr);
                int dc = Math.Sign(tc-fc);
                int r=fr+dr, c=fc+dc;
                while (r!=tr && c!=tc)
                {
                    if (board.Board[r,c]!=null) return false;
                    r+=dr; c+=dc;
                }
                var dest = board.Board[tr,tc];
                if (dest==null || dest.Color!=Color) return true;
            }
            return false;
        }
    }
}