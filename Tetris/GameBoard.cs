using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class GameBoard
    {
        private int rows;
        private int columns;

        public int[,] grid { get; set; }

        public GameBoard(int numRows, int numColumns)
        {
            rows = numRows;
            columns = numColumns;
            grid = new int[rows, columns];
        }

        public int GetCellValue(int row, int column)
        {
            return grid[row, column];
        }

        public void SetCellValue(int row, int column, int value)
        {
            grid[row, column] = value;
        }

        public bool IsCellEmpty(int row, int column)
        {
            return GetCellValue(row, column) == 0;
        }

        public void ClearBoard()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    grid[row, column] = 0;
                }
            }
        }

        public void ClearRow(int row)
        {
            for (int column = 0; column < columns; column++)
            {
                grid[row, column] = 0;
            }
        }
    }
}
