using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.IO;
using System.Linq;

namespace MyApp
{
    public partial class MainWindow : Window
    {
        private readonly GameField _game;
        private Button?[,] _cells = new Button[8,8];

        public MainWindow()
        {
            InitializeComponent();
            _game = new GameField();
            BuildBoardGrid();
            Render();

            BtnNew.Click += (_,__) => { _game.Reset(); Render(); TxtStatus.Text = "Новая игра"; };
            BtnSave.Click += async (_,__) => {
                var dlg = new SaveFileDialog();
                dlg.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "txt" } });
                var path = await dlg.ShowAsync(this);
                if (!string.IsNullOrEmpty(path)) File.WriteAllText(path, _game.SaveToString());
            };
            BtnLoad.Click += async (_,__) => {
                var dlg = new OpenFileDialog();
                dlg.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "txt" } });
                var paths = await dlg.ShowAsync(this);
                if (paths != null && paths.Length>0)
                {
                    var text = File.ReadAllText(paths[0]);
                    _game.LoadFromString(text);
                    Render();
                }
            };
        }

        private void BuildBoardGrid()
        {
            BoardGrid.RowDefinitions.Clear();
            BoardGrid.ColumnDefinitions.Clear();
            for (int i=0;i<8;i++) BoardGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            for (int i=0;i<8;i++) BoardGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

            for (int r=0;r<8;r++)
            for (int c=0;c<8;c++)
            {
                var btn = new Button { Margin = new Thickness(1) };
                btn.Click += CellClick;
                Grid.SetRow(btn, r);
                Grid.SetColumn(btn, c);
                BoardGrid.Children.Add(btn);
                _cells[r,c] = btn;
            }
        }

        private (int r,int c)? _selected = null;

        private void CellClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                int r = Grid.GetRow(btn);
                int c = Grid.GetColumn(btn);
                if (_selected == null)
                {
                    if (_game.Board[r,c] != null && _game.Board[r,c].Color == _game.CurrentTurn)
                    {
                        _selected = (r,c);
                    }
                }
                else
                {
                    var from = _selected.Value;
                    var to = (r,c);
                    var res = _game.TryMove(from.r, from.c, to.r, to.c, out string msg);
                    _selected = null;
                    TxtStatus.Text = msg;
                    Render();
                }
            }
        }

        private void Render()
        {
            for (int r=0;r<8;r++)
            for (int c=0;c<8;c++)
            {
                var btn = _cells[r,c]!;
                var fig = _game.Board[r,c];
                btn.Content = fig?.ToSymbol() ?? "";
                bool dark = (r+c)%2==1;
                btn.Background = dark ? Brushes.SaddleBrown : Brushes.Bisque;
                btn.Foreground = fig?.Color == PieceColor.White ? Brushes.Black : Brushes.White;
            }

            if (_game.IsInCheck(_game.CurrentTurn))
                TxtStatus.Text = $"Шах: {_game.CurrentTurn}";
            else if (_game.IsCheckmate(_game.CurrentTurn))
                TxtStatus.Text = $"МАТ: {_game.CurrentTurn}";
        }
    }
}