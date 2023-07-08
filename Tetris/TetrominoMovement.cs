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
            int[,] temp_piece = new int[tetromino.length, tetromino.length];
            int[][] temp_position = new int[5][];

            for (int i = 0; i < tetromino.length; i++)
                for (int j = 0; j < tetromino.length; j++)
                    temp_piece[j, tetromino.length - 1 - i] = tetromino.piece[i, j];

            for (int i = 0; i < 4; i++)
                temp_position[i] = new int[2] {
                        tetromino.position[i][1] - tetromino.position[4][1] + tetromino.position[4][0],
                        tetromino.length - 1 - tetromino.position[i][0] + tetromino.position[4][0] + tetromino.position[4][1]
                };

            temp_position[4] = new int[2] { tetromino.position[4][0], tetromino.position[4][1] };

            if (temp_position[4][1] == -2) TetrominoMove(temp_position, 0, 2);

            else if (temp_position[4][1] == -1) TetrominoMove(temp_position, 0, 1);

            else if (temp_position[4][1] == 8 && tetromino.length == 4 || tetromino.position[4][1] == 9 && tetromino.length == 3) TetrominoMove(temp_position, 0, -1);

            else if (temp_position[4][1] == 9 && tetromino.length == 4) TetrominoMove(temp_position, 0, -2);

            if (temp_position[4][0] == 19 && tetromino.length == 4 || tetromino.position[4][0] == 20 && tetromino.length == 3) TetrominoMove(temp_position, -1, 0);

            else if (temp_position[4][0] == 20 && tetromino.length == 4) TetrominoMove(temp_position, -2, 0);

            int[,] wallKickOffsets = new int[15, 2] { {0, 0}, {0, -1}, {0, 1}, {0, -2}, {0, 2}, {1, 0}, {1, -1}, {1, 1}, {1, -2}, {1, 2}, {-1, 0}, {-1, -1}, {-1, 1}, {-1, -2}, {-1, 2} };

            for (int a = 0; a < 15; a++)
            {
                TetrominoMove(temp_position, wallKickOffsets[a, 0], wallKickOffsets[a, 1]);

                if (Enumerable.Range(0, 4).All(i => temp_position[i][0] >= 0 && temp_position[i][0] < 22 && temp_position[i][1] >= 0 && temp_position[i][1] < 11)
                    && Enumerable.Range(0, 4).All(i => gameboard[temp_position[i][0], temp_position[i][1]] >= 0))
                {
                    foreach (int i in Enumerable.Range(0, 4))
                        gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 0;

                    for (int i = 0; i < tetromino.length; i++)
                        for (int j = 0; j < tetromino.length; j++)
                            tetromino.piece[i, j] = temp_piece[i, j];

                    for (int i = 0; i < 5; i++)
                    {
                        tetromino.position[i][0] = temp_position[i][0];
                        tetromino.position[i][1] = temp_position[i][1];
                    }

                    foreach (int i in Enumerable.Range(0, 4))
                        gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 1;

                    break;
                }

                TetrominoMove(temp_position, -wallKickOffsets[a, 0], -wallKickOffsets[a, 1]);
            }
        }

        public static void TetrominoInitialize(int[,] gameboard, Tetromino tetromino)
        {
            int c = 0;

            for (int i = 0; i < tetromino.piece.GetLength(0); i++)
            {
                for (int j = 0; j < tetromino.piece.GetLength(1); j++)
                {
                    if (i >= 0 && i < gameboard.GetLength(0) && j + 4 >= 0 && j + 4 < gameboard.GetLength(1) && tetromino.piece[i, j] == 1)
                    {
                        tetromino.position[c][0] = i;
                        tetromino.position[c][1] = j + 4;
                        c++;
                    }
                }
            }

            tetromino.position[4][0] = 0;
            tetromino.position[4][1] = 4;

            if (Enumerable.Range(0, 4).All(i => gameboard[tetromino.position[i][0], tetromino.position[i][1]] == 0))
                foreach (int i in Enumerable.Range(0, 4))
                    gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 1;

            else if (Enumerable.Range(0, 4).All(i => tetromino.position[i][0] - 1 < 0 || gameboard[tetromino.position[i][0] - 1, tetromino.position[i][1]] == 0))
            {
                foreach (int i in Enumerable.Range(0, 5))
                    tetromino.position[i][0] -= 1;

                foreach (int i in Enumerable.Range(0, 4))
                    if (tetromino.position[i][0] >= 0)
                        gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 1;
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

            tetromino.position[4][1] += d;
        }

        public static void TetrominoMove(int[][] position, int y, int x)
        {
            for (int i = 0; i < 5; i++)
            {
                position[i][0] += y;

                position[i][1] += x;
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

            tetromino.position[4][0] += 1;
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

        public static void TetrominoInstaPlace(int[,] gameboard, Tetromino tetromino, int[][] ghostTetromino)
        {
            for (int i = 0; i < 4; i++)
            {
                gameboard[tetromino.position[i][0], tetromino.position[i][1]]--;

                tetromino.position[i][0] = ghostTetromino[i][0];
                tetromino.position[i][1] = ghostTetromino[i][1];

                gameboard[tetromino.position[i][0], tetromino.position[i][1]]++;
            }
        }

        public static void GhostTetrominoLocate(int[,] gameboard, Tetromino tetromino, out int[][] ghostTetromino)
        {
            ghostTetromino = new int[4][];

            int minFallDistance = 21;

            for (int i = 0; i < 4; i++)
            {
                ghostTetromino[i] = new int[2];
                ghostTetromino[i][1] = tetromino.position[i][1];

                int j = tetromino.position[i][0];

                for (; j < 21; j++)
                {
                    if (gameboard[j + 1, tetromino.position[i][1]] < 0)
                        break;
                }

                if (minFallDistance > j - tetromino.position[i][0])
                    minFallDistance = j - tetromino.position[i][0];
            }

            for (int i = 0; i < 4; i++)
            {
                ghostTetromino[i][0] = tetromino.position[i][0] + minFallDistance;
            }
        }

        public static IEnumerable<Vector2> GhostTetrominoDisplay(int[][] ghostTetromino)
        {
            for (int i = 0; i < 4; i++)
            {
                yield return new Vector2(25 + 28 * ghostTetromino[i][1], 31 + 28 * ghostTetromino[i][0]);
            }
        }
    }
}
