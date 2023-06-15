using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal static class TetrominoMovement
    {
        public static void TetrominoRotate(int[,] gameboard, Tetromino tetromino)
        {
            int[,] temp = new int[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    temp[j, 3 - i] = tetromino.piece[i, j];
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    tetromino.piece[i, j] = temp[i, j];
                }
            }
        }

        public static void TetrominoInitialize(int[,] gameboard, Tetromino tetromino)
        {
            int c = 0;

            for (int i = 0; i < tetromino.piece.GetLength(0); i++)
            {
                for (int j = 0; j < tetromino.piece.GetLength(1); j++)
                {
                    int row = i;
                    int column = 4 + j;

                    if (row >= 0 && row < gameboard.GetLength(0) && column >= 0 && column < gameboard.GetLength(1) && tetromino.piece[i, j] == 1)
                    {
                        gameboard[row, column] = tetromino.piece[i, j];

                        tetromino.position[c][0] = row;
                        tetromino.position[c][1] = column;
                        c++;
                    }
                }
            }
        }

        public static void TetrominoMove(int[,] gameboard, Tetromino tetromino, int d)
        {
            for (int i = 0; i < 4; i++)
            {
                gameboard[tetromino.position[i][0], tetromino.position[i][1]]--;

                gameboard[tetromino.position[i][0], tetromino.position[i][1] + d]++;

                tetromino.position[i][1] += d;
            }
        }

        public static void TetrominoDrop(int[,] gameboard, Tetromino tetromino)
        {
            for (int i = 0; i < 4; i++)
            {
                gameboard[tetromino.position[i][0], tetromino.position[i][1]]--;

                gameboard[tetromino.position[i][0] + 1, tetromino.position[i][1]]++;

                tetromino.position[i][0] += 1;
            }
        }

        public static void TetrominoLand(int[,] gameboard, Tetromino tetromino)
        {
            int c = 0;

            if (tetromino.color == Color.Yellow) c = 1;

            else if (tetromino.color == Color.Blue) c = 2;

            else if (tetromino.color == Color.Green) c = 3;

            else if (tetromino.color == Color.Red) c = 4;

            else if (tetromino.color == Color.Orange) c = 5;

            else if (tetromino.color == Color.Purple) c = 6;

            else if (tetromino.color == Color.DeepPink) c = 7;

            for (int i = 0; i < 4; i++)
            {
                gameboard[tetromino.position[i][0], tetromino.position[i][1]] = -c;
            }
        }
    }
}
