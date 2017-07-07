using System;

namespace Minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            bool keepPlaying = true;
            while (keepPlaying)
            {
                var board = Board.CreateBoard();
                board.Play();
                string choice = " ";
                while (!"YN".Contains(choice))
                {
                    Console.Write("Play again? (Y/N) ");
                    var input = Console.ReadKey();
                    Console.WriteLine();
                    choice = input.KeyChar.ToString().ToUpperInvariant();
                    switch (choice)
                    {
                        case "Y": keepPlaying = true; break;
                        case "N": keepPlaying = false; break;
                    }
                }
            }
        }
    }
}