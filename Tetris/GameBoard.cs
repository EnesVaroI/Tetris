using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class GameBoard
    {
        private readonly int rows;
        private readonly int columns;

        public int[,] grid { get; set; }

        public GameBoard(int numRows, int numColumns)
        {
            rows = numRows;
            columns = numColumns;
            grid = new int[rows, columns];
        }

        public int this[int row, int column]
        {
            get => grid[row, column];

            set => grid[row, column] = value;
        }

        public void ClearBoard()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    this[row, column] = 0;
                }
            }
        }

        public void ClearRow(int row)
        {
            for (int column = 0; column < columns; column++)
            {
                this[row, column] = 0;
            }

            LineClearGravity(row);
        }

        public bool IsRowFull(int row)
        {
            for (int column = 0; column < columns; column++)
            {
                if (this[row, column] >= 0)
                    return false;
            }

            return true;
        }

        private void LineClearGravity(int row)
        {
            for (row--; row >= 0; row--)
                for (int column = 0; column < columns; column++)
                {
                    this[row + 1, column] = grid[row, column];
                    this[row, column] = 0;
                }
        }
    }
}
