using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using SudokuGenerator.Annotations;

namespace SudokuGenerator.Core
{
    public class Number : INotifyPropertyChanged
    {
        #region Constructor

        public Number(int row, int column)
        {
            Value = null;
            Row = row;
            Column = column;
            Known = false;
            PossibleNumbers = new Dictionary<int, bool>(9);
            for (var index = 1; index <= 9; index++)
            {
                PossibleNumbers.Add(index, true);
            }
        }

        #endregion

        #region Model & Data

        private int? _value;

        public int? Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public int Row;
        public int Column;
        public bool Known;
        public Dictionary<int, bool> PossibleNumbers;

        #endregion

        #region ViewModel

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class Sudoku : INotifyPropertyChanged
    {
        #region Constructor

        public Sudoku()
        {
            Numbers = new ObservableCollection<Number>();
            for (var row = 0; row < 9; row++)
            {
                for (var column = 0; column < 9; column++)
                {
                    Numbers.Add(new Number(row, column));
                }
            }
        }

        #endregion

        #region Model & Data

        private ObservableCollection<Number> _numbers;

        public ObservableCollection<Number> Numbers
        {
            get { return _numbers; }
            set
            {
                if (_numbers == value) return;
                _numbers = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region ViewModel

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Solving

        public void Solve()
        {
            var history = new List<int>();
            var rand = new Random();
            for (var numberIndex = 0; numberIndex < 81; numberIndex++)
            {
                if (Numbers[numberIndex].Known) continue;
                var currentNumber = Numbers[numberIndex];
                for (var index = 1; index <= 9; index++)
                {
                    if (Numbers[numberIndex].PossibleNumbers[index] != true) continue;
                    if (!Validate(currentNumber.Row, currentNumber.Column, index))
                    {
                        Numbers[numberIndex].PossibleNumbers[index] = false;
                    }
                }
                fallback:
                var possibleNumbers = from number in Numbers[numberIndex].PossibleNumbers
                    where number.Value
                    select number.Key;
                var possibleNumbersArray = possibleNumbers.ToArray();
                if (possibleNumbersArray.Any())
                {
                    var resultNumber = possibleNumbersArray[rand.Next(possibleNumbersArray.Count())];
                    Numbers[numberIndex].Value = resultNumber;
                    history.Add(numberIndex);
                }
                else
                {
                    ClearNow(numberIndex);
                    numberIndex = history[history.Count - 1];
                    history.RemoveAt(history.Count - 1);
                    if (history.Count == 0)
                    {
                        PromptNoAnswers();
                        return;
                    }
                    RemoveLast(numberIndex);
                    goto fallback;
                }
            }
        }


        private void ClearNow(int numberIndex)
        {
            for (var index = 1; index <= 9; index++)
            {
                Numbers[numberIndex].PossibleNumbers[index] = true;
            }
            Numbers[numberIndex].Value = 0;
        }

        private void RemoveLast(int numberIndex)
        {
            Numbers[numberIndex].PossibleNumbers[Numbers[numberIndex].Value.GetValueOrDefault()] = false;
            Numbers[numberIndex].Value = 0;
        }

        private static async void PromptNoAnswers()
        {
            var resLoader = new ResourceLoader();
            var message = resLoader.GetString("NoAnswers");
            var noAnswersDialog = new MessageDialog(message);
            await noAnswersDialog.ShowAsync();
        }

        #endregion

        #region Validation

        public bool Validate(Number number)
        {
            return ValidateRow(number)
                   && ValidateColumn(number)
                   && ValidateGrid(number);
        }

        public bool Validate(int row, int column, int value)
        {
            var number = new Number(row, column) {Value = value};
            return ValidateRow(number)
                   && ValidateColumn(number)
                   && ValidateGrid(number);
        }

        private bool ValidateRow(Number number)
        {
            for (var column = 0; column < 9; column++)
            {
                var numberIndex = number.Row*9 + column;
                if (Numbers[numberIndex].Value.GetValueOrDefault() == number.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateColumn(Number number)
        {
            for (var row = 0; row < 9; row++)
            {
                var numberIndex = row*9 + number.Column;
                if (Numbers[numberIndex].Value.GetValueOrDefault() == number.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateGrid(Number number)
        {
            var gridRow = number.Row/3;
            var gridColumn = number.Column/3;
            for (var row = 3*gridRow; row < 3*gridRow + 3; row++)
            {
                for (var column = 3*gridColumn; column < 3*gridColumn + 3; column++)
                {
                    var numberIndex = row*9 + column;
                    if (Numbers[numberIndex].Value.GetValueOrDefault() == number.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }
}