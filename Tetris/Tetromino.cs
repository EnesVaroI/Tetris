using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Tetris
{
    internal class Tetromino
    {
        public enum Shape
        {
            O,
            I,
            T,
            L,
            J,
            S,
            Z
        }

        public Shape shape { get; private set; }

        public Tetromino()
        {
            Random random = new Random();
            shape = (Shape)random.Next(0, 7);
            
            piece = pieceSelection(shape);
            color = colorSelection(shape);
            length = piece.GetLength(0);
        }

        public Tetromino(Tetromino tetromino)
        {
            this.shape = tetromino.shape;
            this.piece = tetromino.piece;
            this.color = tetromino.color;
            this.position = tetromino.position;
        }

        public Tetromino(Shape shape)
        {
            this.shape = shape;
            this.piece = pieceSelection(shape);
            this.color = colorSelection(shape);
            this.length = piece.GetLength(0);
        }

        private readonly int[,] piece0 = {
            { 1, 1 },
            { 1, 1 }
        };

        private readonly int[,] piece1 = {
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        };

        private readonly int[,] piece2 = {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };

        private readonly int[,] piece3 = {
            { 0, 0, 1 },
            { 1, 1, 1 },
            { 0, 0, 0 }
        };

        private readonly int[,] piece4 = {
            { 1, 0, 0 },
            { 1, 1, 1 },
            { 0, 0, 0 }
        };

        private readonly int[,] piece5 = {
            { 0, 1, 1 },
            { 1, 1, 0 },
            { 0, 0, 0 }
        };

        private readonly int[,] piece6 = {
            { 1, 1, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 }
        };

        public int[,] piece { get; set; }

        public Color color { get; private set; }

        public int[][] position { get; set; } = new int[][] {
            new int[2],
            new int[2],
            new int[2],
            new int[2],
            new int[2]
        };

        public int length { get; private set; }

        private int[,] pieceSelection(Shape shape)
        {
            switch (shape)
            {
                case Shape.O:
                    return piece0;

                case Shape.I:
                    return piece1;

                case Shape.T:
                    return piece2;

                case Shape.L:
                    return piece3;

                case Shape.J:
                    return piece4;

                case Shape.S:
                    return piece5;

                case Shape.Z:
                    return piece6;

                default:
                    return null;
            }
        }

        public Color colorSelection(Shape shape)
        {
            return shape switch
            {
                Shape.O => Color.Yellow,
                Shape.I => Color.Blue,
                Shape.T => Color.Green,
                Shape.L => Color.Red,
                Shape.J => Color.Orange,
                Shape.S => Color.Purple,
                Shape.Z => Color.DeepPink,
                _ => Color.White
            };
        }

        public (int Min, int Max) x_position()
        {
            int min = position[0][1], max = position[0][1];

            for (int i = 0; i < 3; i++)
            {
                if (position[i + 1][1] > max)
                    max = position[i + 1][1];

                if (position[i + 1][1] < min)
                    min = position[i + 1][1];
            }

            return (min, max);
        }

        public (int Min, int Max) y_position()
        {
            int min = position[0][0], max = position[0][0];

            for (int i = 0; i < 3; i++)
            {
                if (position[i + 1][0] > max)
                    max = position[i + 1][0];

                if (position[i + 1][0] < min)
                    min = position[i + 1][0];
            }

            return (min, max);
        }

        public bool isLanded(int[,] gameboard) => y_position().Max == 21 || Enumerable.Range(0, 4).Any(i => gameboard[position[i][0] + 1, position[i][1]] < 0);
        public bool isLeftmost(int[,] gameboard) => x_position().Min == 0 || Enumerable.Range(0, 4).Any(i => gameboard[position[i][0], position[i][1] - 1] < 0);
        public bool isRightmost(int[,] gameboard) => x_position().Max == 10 || Enumerable.Range(0, 4).Any(i => gameboard[position[i][0], position[i][1] + 1] < 0);
    }
}
