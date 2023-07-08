using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Tetromino
    {
        enum Shape
        {
            straight,
            square,
            t,
            l,
            skew
        }

        public Tetromino()
        {
            Random random = new Random();
            int i = random.Next(0, 7);
            
            piece = pieceSelection(i);
            color = colorSelection(i);
            length = piece.GetLength(0);
        }

        public Tetromino(Tetromino tetromino)
        {
            this.piece = tetromino.piece;
            this.color = tetromino.color;
            this.position = tetromino.position;
        }

        public Tetromino(int i)
        {
            piece = pieceSelection(i);
            color = colorSelection(i);
            length = piece.GetLength(0);
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

        public int length { get; set; }

        private int[,] pieceSelection(int i)
        {
            switch (i)
            {
                case 0:
                    return piece0;

                case 1:
                    return piece1;

                case 2:
                    return piece2;

                case 3:
                    return piece3;

                case 4:
                    return piece4;

                case 5:
                    return piece5;

                case 6:
                    return piece6;

                default:
                    return null;
            }
        }

        public Color colorSelection(int i)
        {
            return i switch
            {
                0 => Color.Yellow,
                1 => Color.Blue,
                2 => Color.Green,
                3 => Color.Red,
                4 => Color.Orange,
                5 => Color.Purple,
                6 => Color.DeepPink,
                _ => Color.White
            };
        }

        public int getPieceType()
        {
            List<int[,]> pieces = new List<int[,]> { piece0, piece1, piece2, piece3, piece4, piece5, piece6 };
            return pieces.IndexOf(piece);
        }

        public ValueTuple<int, int> x_position()
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

        public ValueTuple<int, int> y_position()
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
    }
}
