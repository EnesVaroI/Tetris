using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    internal static class TetrominoMovement
    {
        public enum RotationDirection
        {
            Clockwise,
            CounterClockwise
        }

        public static void TetrominoRotate(int[,] gameboard, Tetromino tetromino, RotationDirection d)
        {
            int[,] temp_piece = new int[tetromino.length, tetromino.length];
            int[][] temp_position = new int[5][];

            switch (d)
            {
                case RotationDirection.Clockwise:
                    RotateAndReposition(
                        (i, j) => (j, tetromino.length - 1 - i),
                        (x, y) => (x - tetromino.position[4][1] + tetromino.position[4][0], tetromino.length - 1 - y + tetromino.position[4][0] + tetromino.position[4][1])
                    );
                    tetromino.rotation = (tetromino.rotation + 1) % 4;
                    break;

                case RotationDirection.CounterClockwise:
                    RotateAndReposition(
                        (i, j) => (tetromino.length - 1 - j, i),
                        (x, y) => (tetromino.length - 1 - x + tetromino.position[4][1] + tetromino.position[4][0], y - tetromino.position[4][0] + tetromino.position[4][1])
                    );
                    tetromino.rotation = (tetromino.rotation + 3) % 4;
                    break;

                default:
                    throw new ArgumentException($"Invalid rotation direction: {d}");
            }

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

            void RotateAndReposition(Func<int, int, (int, int)> Rotate, Func<int, int, (int, int)> Reposition)
            {
                for (int i = 0; i < tetromino.length; i++)
                    for (int j = 0; j < tetromino.length; j++)
                    {
                        var (x, y) = Rotate(i, j);
                        temp_piece[x, y] = tetromino.piece[i, j];
                    }

                for (int i = 0; i < 4; i++)
                {
                    var (x, y) = Reposition(tetromino.position[i][1], tetromino.position[i][0]);
                    temp_position[i] = new int[2] { x, y };
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

            else if (Enumerable.Range(0, 4).All(i => tetromino.position[i][0] - 1 >= 0 && gameboard[tetromino.position[i][0] - 1, tetromino.position[i][1]] == 0))
            {
                foreach (int i in Enumerable.Range(0, 5))
                    tetromino.position[i][0] -= 1;

                foreach (int i in Enumerable.Range(0, 4))
                    if (tetromino.position[i][0] >= 0)
                        gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 1;
            }

            else
                throw new GameOverException("Game Over: Failed to create a new Tetromino due to lack of space.");
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

            tetromino.position[4][0] = ghostTetromino[4][0];
            tetromino.position[4][1] = ghostTetromino[4][1];
        }

        public static void GhostTetrominoLocate(int[,] gameboard, Tetromino tetromino, out int[][] ghostTetromino)
        {
            ghostTetromino = new int[5][];

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

            ghostTetromino[4] = new int[2];
            ghostTetromino[4][1] = tetromino.position[4][1];

            for (int i = 0; i < 5; i++)
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

        public static int CalculateHeight(int[,] gameboard, int x_coord)
        {
            x_coord = (x_coord == -1) ? 0 : (x_coord == 11) ? 10 : x_coord;

            int i = 0;
            for (; i < 22; i++)
            {
                if (i == 21)
                    return 0;

                if (gameboard[i, x_coord] == 1 && gameboard[i + 1, x_coord] <= 0)
                    break;
            }

            int j = i;
            for (; j < 22; j++)
            {
                if (gameboard[j, x_coord] < 0)
                    break;
            }

            return j - i - 1;
        }

        public static int[] CalculateGroundState(int[,] gameboard, int x_coord)
        {
            x_coord = (x_coord == -1) ? 0 : (x_coord == 11) ? 10 : x_coord;

            int leftHeight = 22, middleHeight = 22, rightHeight = 22;

            for (int i = 0; i < 22; i++)
            {
                if (x_coord - 1 >= 0 && gameboard[i, x_coord - 1] < 0)
                {
                    leftHeight = 22 - i;
                    break;
                }
                else if (i == 21)
                    leftHeight = 0;
            }

            for (int i = 0; i < 22; i++)
            {
                if (gameboard[i, x_coord] < 0)
                {
                    middleHeight = 22 - i;
                    break;
                }
                else if (i == 21)
                    middleHeight = 0;
            }

            for (int i = 0; i < 22; i++)
            {
                if (x_coord + 1 < 11 && gameboard[i, x_coord + 1] < 0)
                {
                    rightHeight = 22 - i;
                    break;
                }
                else if (i == 21)
                    rightHeight = 0;
            }

            return new int[2] { middleHeight - leftHeight, middleHeight - rightHeight };
        }
    }

    internal class GameOverException : Exception
    {
        public GameOverException(string message) : base(message)
        {
        }
    }
}