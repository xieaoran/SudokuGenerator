using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SudokuGenerator.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SudokuGenerator
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Sudoku _newSudoku;

        public MainPage()
        {
            InitializeComponent();
            LoadEmptySudoku();
            ClearButton.IsEnabled = false;
            SolveButton.IsEnabled = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            LoadEmptySudoku();
            SudokuView.IsEnabled = true;
            ClearButton.IsEnabled = false;
            SolveButton.IsEnabled = true;
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            SudokuView.SelectedItem = null;
            _newSudoku.Solve();
            SudokuView.IsEnabled = false;
            ClearButton.IsEnabled = true;
            SolveButton.IsEnabled = false;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void LoadEmptySudoku()
        {
            _newSudoku = new Sudoku();
            SudokuView.DataContext = _newSudoku;
        }

        private void KeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            if (SudokuView.SelectedItems.Count == 0) return;
            var selectedNumber = ((Number) SudokuView.SelectedItem);
            var keyNumber = 0;
            switch (e.Key)
            {
                case VirtualKey.Number1:
                    keyNumber = 1;
                    break;
                case VirtualKey.Number2:
                    keyNumber = 2;
                    break;
                case VirtualKey.Number3:
                    keyNumber = 3;
                    break;
                case VirtualKey.Number4:
                    keyNumber = 4;
                    break;
                case VirtualKey.Number5:
                    keyNumber = 5;
                    break;
                case VirtualKey.Number6:
                    keyNumber = 6;
                    break;
                case VirtualKey.Number7:
                    keyNumber = 7;
                    break;
                case VirtualKey.Number8:
                    keyNumber = 8;
                    break;
                case VirtualKey.Number9:
                    keyNumber = 9;
                    break;
                case VirtualKey.Delete:
                    ((Number) SudokuView.SelectedItem).Value = null;
                    ((Number) SudokuView.SelectedItem).Known = false;
                    return;
                default:
                    break;
            }
            if (keyNumber >= 1
                && keyNumber <= 9
                && _newSudoku.Validate(selectedNumber.Row, selectedNumber.Column, keyNumber))
            {
                ((Number) SudokuView.SelectedItem).Value = keyNumber;
                ((Number) SudokuView.SelectedItem).Known = true;
            }
        }
    }
}