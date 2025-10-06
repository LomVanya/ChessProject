using System;

namespace MyApp
{
    public abstract class Figure
    {
        public PieceColor Color { get; set; }
        public Figure(PieceColor color) { Color = color; }

        // Виртуальная функция проверки допустимости хода (без учёта шаха)
        public abstract bool IsValidMove(GameField board, int fromR, int fromC, int toR, int toC);

        // Вывод фигуры на экран: краткая символьная форма
        public abstract char ToChar();

        public string ToSymbol() => ToChar().ToString();
    }
}