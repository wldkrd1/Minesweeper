using System;
using System.Collections.Generic;

namespace Minesweeper
{
    class Board
    {
        bool _hasLost = false;
        List<Tuple<int, int>> _bombLocations;
        Cell[][] _board;
        int _sizeOfBoard;
        int _flagged;
        Queue<Tuple<int, int>> _toCheck;
        public static Board CreateBoard()
        {
            var n = GetN();
            long numBombs = GetNumBombs(n);
            return new Board(n, numBombs);
        }

        private static long GetNumBombs(int n)
        {
            Int64 numBombs = 0;
            while (numBombs < 1 || numBombs > n * n)
            {
                Console.Write("How many bombs to hide? ");
                var input = Console.ReadLine();
                if (!Int64.TryParse(input, out numBombs) || numBombs < 1 || numBombs > n * n)
                    Console.WriteLine($"Number of bombs must be between 1 and {n * n}!");
            }
            return numBombs;
        }

        private static int GetN()
        {
            int n = 0;
            while (n < 1 || n > 1000)
            {
                Console.Clear();
                Console.Write("Creating an N by N board. Please enter N: ");
                var input = Console.ReadLine();
                if (!Int32.TryParse(input, out n) || n < 1 || n > 1000)
                    Console.WriteLine("N must be between 1 and 1000!");
            }
            return n;
        }
        
        public Board(int SizeOfBoard, Int64 NumBombsToHide)
        {
            _toCheck = new Queue<Tuple<int, int>>();
            _bombLocations = new List<Tuple<int, int>>(NumBombsToHide > int.MaxValue ? int.MaxValue : ((int)NumBombsToHide));
            _sizeOfBoard = SizeOfBoard;
            _board = new Cell[SizeOfBoard][];
            for (int i = 0; i < SizeOfBoard; i++)
            {
                _board[i] = new Cell[SizeOfBoard];
                for (int j = 0; j < SizeOfBoard; j++)
                {
                    _board[i][j] = new Cell();
                }
            }
            Random random = new Random();
            while (NumBombsToHide > 0)
            {
                int randomx = random.Next(0, SizeOfBoard);
                int randomy = random.Next(0, SizeOfBoard);
                if (!_board[randomx][randomy].HasBomb)
                {
                    AddBomb(randomx, randomy);
                    NumBombsToHide--;
                    _bombLocations.Add(new Tuple<int, int>(randomx, randomy));
                }
            }
        }
        
        public void Play()
        {
            DrawBoard();
            while (!(_hasWon | _hasLost))
            {
                string choice = " ";
                while (!"SFQ".Contains(choice))
                {
                    Console.Write("(S)how, (F)lag or (Q)uit? ");
                    var input = Console.ReadKey();
                    Console.WriteLine();
                    choice = input.KeyChar.ToString().ToUpperInvariant();
                }
                if (choice == "Q")
                    break;
                int x = 0;
                int y = 0;
                while (y < 1 || y > _sizeOfBoard)
                {
                    Console.Write("Please enter x coordinate: ");
                    var yInput = Console.ReadLine();
                    if (!Int32.TryParse(yInput, out y) || y < 1 || y > _sizeOfBoard) //yes, I know these are reversed
                        Console.WriteLine($"X must be between 1 and {_sizeOfBoard}!");
                }
                while (x < 1 || x > _sizeOfBoard)
                {
                    Console.Write("Please enter y coordinate: ");
                    var xInput = Console.ReadLine();
                    if (!Int32.TryParse(xInput, out x) || x < 1 || x > _sizeOfBoard)   //yes, I know these are reversed
                        Console.WriteLine($"Y must be between 1 and {_sizeOfBoard}!");
                }
                switch (choice)
                {
                    case "S":
                        Show(x, y);
                        break;
                    case "F":
                        Flag(x, y);
                        break;
                }
                DrawBoard();
                if (_hasWon)
                {
                    Console.WriteLine("You win!");
                    break;
                }
                if (_hasLost)
                {
                    Console.WriteLine("You lose!");
                    break;
                }
            }
        }
        
        private void AddBomb(int x, int y)
        {
            _board[x][y].HasBomb = true;
            UpdateCount(x - 1, y - 1);
            UpdateCount(x, y - 1);
            UpdateCount(x + 1, y - 1);
            UpdateCount(x - 1, y);
            UpdateCount(x + 1, y);
            UpdateCount(x - 1, y + 1);
            UpdateCount(x, y + 1);
            UpdateCount(x + 1, y + 1);
        }
        
        private void UpdateCount(int x, int y)
        {
            if (x >= 0 && x < _sizeOfBoard && y >= 0 && y < _sizeOfBoard)
                _board[x][y].Count++;
        }
        
        private void DrawBoard()
        {
               Console.Clear();
            var origColor = Console.ForegroundColor;
            Console.WriteLine($"Bombs Remaining: {_bombLocations.Count - _flagged}");
            Console.ForegroundColor = ConsoleColor.White;
            StringBuilder sbFormat = new StringBuilder("  ");
            List<string> sbLine = new List<string>(_sizeOfBoard);
            for (int x = 0; x < _sizeOfBoard; x++)
            {
                sbFormat.Append("{" + x + ",2}");
                sbLine.Add((x + 1).ToString());
            }
            Console.WriteLine(string.Format(sbFormat.ToString(), sbLine.ToArray()));
            for (int i = 0; i < _sizeOfBoard; i++)
            {
                Console.Write("  ");
                for (int x = 0; x < _sizeOfBoard; x++)
                {
                    Console.Write("--");
                }
                Console.WriteLine("-");
                Console.Write(String.Format("{0,2}", i + 1));
                for (int j = 0; j < _sizeOfBoard; j++)
                {
                    Console.Write("|");
                    switch (_board[i][j].State)
                    {
                        case DisplayState.Flagged: Console.ForegroundColor = ConsoleColor.Yellow; break;
                        case DisplayState.Hidden: Console.ForegroundColor = ConsoleColor.Gray; break;
                        case DisplayState.Shown: if (_board[i][j].HasBomb) Console.ForegroundColor = ConsoleColor.Red; else Console.ForegroundColor = ConsoleColor.Green; break;
                    }
                    Console.Write(_board[i][j]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine("|");
            }
            Console.Write("  ");
            for (int x = 0; x < _sizeOfBoard; x++)
            {
                Console.Write("--");
            }
            Console.WriteLine("-");
            Console.ForegroundColor = origColor;
        }
        
        private void Flag(int x, int y)
        {
            x--;
            y--;
            if (x >= 0 && x < _sizeOfBoard && y >= 0 && y < _sizeOfBoard)
            {
                switch (_board[x][y].State)
                {
                    case DisplayState.Flagged:
                        if (_flagged > 0)
                        {
                            _flagged--;
                            _board[x][y].State = DisplayState.Hidden;
                        }
                        break;
                    case DisplayState.Hidden:
                        if (_flagged < _bombLocations.Count)
                        {
                            _flagged++;
                            _board[x][y].State = DisplayState.Flagged;
                        }
                        break;
                    case DisplayState.Shown: break;
                }
            }
        }
        
        private void Show(int x, int y)
        {
            x--;
            y--;
            if (x >= 0 && x < _sizeOfBoard && y >= 0 && y < _sizeOfBoard)
            {
                if (_board[x][y].HasBomb)
                {
                    _hasLost = true;
                    //losing condition
                    foreach (var location in _bombLocations)
                    {
                        _board[location.Item1][location.Item2].State = DisplayState.Shown;
                    }
                }
                else
                {
                    _toCheck.Enqueue(new Tuple<int, int>(x, y));
                    while (_toCheck.Count > 0)
                    {
                        var next = _toCheck.Dequeue();
                        ShowEmptyCells(next.Item1, next.Item2);
                    }
                }
            }
        }
        
        private void ShowEmptyCells(int x, int y)
        {
            if (x >= 0 && x < _sizeOfBoard && y >= 0 && y < _sizeOfBoard)
            {
                if (!_board[x][y].HasBomb && _board[x][y].State == DisplayState.Hidden)
                {
                    _board[x][y].State = DisplayState.Shown;
                    if (_board[x][y].Count == 0)
                    {
                        _toCheck.Enqueue(new Tuple<int, int>(x - 1, y - 1));
                        _toCheck.Enqueue(new Tuple<int, int>(x - 1, y - 1));
                        _toCheck.Enqueue(new Tuple<int, int>(x, y - 1));
                        _toCheck.Enqueue(new Tuple<int, int>(x + 1, y - 1));
                        _toCheck.Enqueue(new Tuple<int, int>(x - 1, y));
                        _toCheck.Enqueue(new Tuple<int, int>(x + 1, y));
                        _toCheck.Enqueue(new Tuple<int, int>(x - 1, y + 1));
                        _toCheck.Enqueue(new Tuple<int, int>(x, y + 1));
                        _toCheck.Enqueue(new Tuple<int, int>(x + 1, y + 1));
                    }
                }
            }
        }
        
        private bool _hasWon
        {
            get
            {
                if (_hasLost)
                    return false;
                for (int i = 0; i < _sizeOfBoard; i++)
                    for (int j = 0; j < _sizeOfBoard; j++)
                    {
                        if (_board[i][j].State == DisplayState.Hidden)
                            return false;
                    }
                return true;
            }
        }
    }
}
