using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MyApp
{
    public class GameField
    {
        public Figure?[,] Board { get; private set; } = new Figure[8,8];
        public PieceColor CurrentTurn { get; private set; } = PieceColor.White;

        public GameField() { Reset(); }

        public void Reset()
        {
            Board = new Figure[8,8];
            // place pawns
            for (int c=0;c<8;c++) { Board[1,c] = new Pawn(PieceColor.Black); Board[6,c] = new Pawn(PieceColor.White); }
            // rooks
            Board[0,0]=new Rook(PieceColor.Black); Board[0,7]=new Rook(PieceColor.Black);
            Board[7,0]=new Rook(PieceColor.White); Board[7,7]=new Rook(PieceColor.White);
            // knights
            Board[0,1]=new Knight(PieceColor.Black); Board[0,6]=new Knight(PieceColor.Black);
            Board[7,1]=new Knight(PieceColor.White); Board[7,6]=new Knight(PieceColor.White);
            // bishops
            Board[0,2]=new Bishop(PieceColor.Black); Board[0,5]=new Bishop(PieceColor.Black);
            Board[7,2]=new Bishop(PieceColor.White); Board[7,5]=new Bishop(PieceColor.White);
            // queens
            Board[0,3]=new Queen(PieceColor.Black); Board[7,3]=new Queen(PieceColor.White);
            // kings
            Board[0,4]=new King(PieceColor.Black); Board[7,4]=new King(PieceColor.White);
            CurrentTurn = PieceColor.White;
        }

        // Попытка сделать ход — возвращает true если успешно
        public bool TryMove(int fr, int fc, int tr, int tc, out string message)
        {
            message = "";
            var piece = Board[fr,fc];
            if (piece==null) { message = "Нет фигуры в указанной клетке."; return false; }
            if (piece.Color != CurrentTurn) { message = "Ход не вашей стороны."; return false; }
            if (!piece.IsValidMove(this, fr, fc, tr, tc)) { message = "Недопустимый ход для этой фигуры."; return false; }

            // создаём копию поля для проверки на шах
            var backup = CloneBoard();
            var captured = Board[tr,tc];
            Board[tr,tc] = piece;
            Board[fr,fc] = null;

            if (IsInCheck(CurrentTurn))
            {
                // откатываем
                Board = backup;
                message = "Ход оставляет короля под боем (нельзя).";
                return false;
            }

            if (captured!=null) message = $"Съедена фигура: {captured.ToSymbol()}";

            // смена хода
            CurrentTurn = CurrentTurn==PieceColor.White ? PieceColor.Black : PieceColor.White;

            // проверяем шах/мат
            if (IsInCheck(CurrentTurn)) message += $" Шах {CurrentTurn}.";
            if (IsCheckmate(CurrentTurn)) message += $" Мат {CurrentTurn}.";

            return true;
        }

        private Figure?[,] CloneBoard()
        {
            var nb = new Figure[8,8];
            for (int r=0;r<8;r++)
            for (int c=0;c<8;c++) nb[r,c] = Board[r,c]; // shallow copy (figures are immutable enough for our purposes)
            return nb;
        }

        public bool IsInCheck(PieceColor color)
        {
            // найти короля
            (int r,int c) king = (-1,-1);
            for (int i=0;i<8;i++) for (int j=0;j<8;j++) if (Board[i,j] is King k && k.Color==color) king=(i,j);
            if (king.r==-1) return false;
            // проверить может ли какая-нибудь вражеская фигура побить короля
            for (int i=0;i<8;i++) for (int j=0;j<8;j++)
            {
                var f = Board[i,j];
                if (f!=null && f.Color!=color)
                {
                    if (f.IsValidMove(this, i, j, king.r, king.c)) return true;
                }
            }
            return false;
        }

        public bool IsCheckmate(PieceColor color)
        {
            if (!IsInCheck(color)) return false;
            // если у стороны нет ни одного легального хода, мат
            for (int fr=0;fr<8;fr++) for (int fc=0;fc<8;fc++)
            {
                var f = Board[fr,fc];
                if (f==null || f.Color!=color) continue;
                for (int tr=0;tr<8;tr++) for (int tc=0;tc<8;tc++)
                {
                    if (!f.IsValidMove(this, fr, fc, tr, tc)) continue;
                    var backup = CloneBoard();
                    var piece = Board[fr,fc];
                    var captured = Board[tr,tc];
                    Board[tr,tc] = piece;
                    Board[fr,fc] = null;
                    bool inCheck = IsInCheck(color);
                    Board = backup;
                    if (!inCheck) return false; // есть спасительный ход
                }
            }
            return true;
        }

        // Сохранение в строку (простая сериализация)
        public string SaveToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(CurrentTurn==PieceColor.White?"w":"b");
            for (int r=0;r<8;r++)
            {
                for (int c=0;c<8;c++)
                {
                    var f = Board[r,c];
                    if (f==null) sb.Append('.');
                    else
                    {
                        char ch = f.ToChar();
                        if (f.Color==PieceColor.White) ch = char.ToUpper(ch);
                        else ch = char.ToLower(ch);
                        sb.Append(ch);
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void LoadFromString(string s)
        {
            var lines = s.Split(new[] { "\r\n","\n" }, StringSplitOptions.None).Where(l=>l.Length>0).ToArray();
            if (lines.Length<9) return;
            CurrentTurn = lines[0].Trim()=="w"?PieceColor.White:PieceColor.Black;
            for (int r=0;r<8;r++)
            for (int c=0;c<8;c++)
            {
                char ch = lines[1+r][c];
                Board[r,c] = CharToPiece(ch);
            }
        }

        private Figure? CharToPiece(char ch)
        {
            if (ch=='.') return null;
            PieceColor col = char.IsUpper(ch)?PieceColor.White:PieceColor.Black;
            ch = char.ToUpper(ch);
            return ch switch
            {
                'P' => new Pawn(col),
                'R' => new Rook(col),
                'N' => new Knight(col),
                'B' => new Bishop(col),
                'Q' => new Queen(col),
                'K' => new King(col),
                _ => null
            };
        }
    }
}