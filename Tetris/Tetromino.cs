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

        private readonly int[,] piece0 = {
            { 0, 0, 0, 0 },
            { 0, 1, 1, 0 },
            { 0, 1, 1, 0 },
            { 0, 0, 0, 0 },
        };

        private readonly int[,] piece1 = {
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
        };

        private readonly int[,] piece2 = {
            { 0, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 1, 1, 1, 0 },
            { 0, 0, 0, 0 },
        };

        private readonly int[,] piece3 = {
            { 0, 0, 0, 0 },
            { 0, 0, 1, 0 },
            { 1, 1, 1, 0 },
            { 0, 0, 0, 0 },
        };

        private readonly int[,] piece4 = {
            { 0, 0, 0, 0 },
            { 1, 0, 0, 0 },
            { 1, 1, 1, 0 },
            { 0, 0, 0, 0 },
        };

        private readonly int[,] piece5 = {
            { 0, 0, 0, 0 },
            { 0, 1, 1, 0 },
            { 1, 1, 0, 0 },
            { 0, 0, 0, 0 },
        };

        private readonly int[,] piece6 = {
            { 0, 0, 0, 0 },
            { 1, 1, 0, 0 },
            { 0, 1, 1, 0 },
            { 0, 0, 0, 0 },
        };

        public int[,] piece { get; set; }

        public Color color { get; private set; }

        public int[][] position { get; set; } = new int[][] {
            new int[2],
            new int[2],
            new int[2],
            new int[2]
        };

        public int[] start_position { get; set; } = new int[2];

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

                default: return null;
            }
        }

        public Color colorSelection(int i)
        {
            switch (i)
            {
                case 0:
                    return Color.Yellow;

                case 1:
                    return Color.Blue;

                case 2:
                    return Color.Green;

                case 3:
                    return Color.Red;

                case 4:
                    return Color.Orange;

                case 5:
                    return Color.Purple;

                case 6:
                    return Color.DeepPink;

                default: return Color.White;
            }
        }

        public Tetromino()
        {
            Random random = new Random();
            int i = random.Next(0, 7);

            piece = pieceSelection(i);
            color = colorSelection(i);
        }

        public ValueTuple<int, int> x_position()
        {
            int min = position[0][1], max = position[0][1];

            for (int i = 0; i < 3; i++)
            {
                if (position[i + 1][1] > position[i][1])
                    max = position[i + 1][1];

                if (position[i + 1][1] < position[i][1])
                    min = position[i + 1][1];
            }

            return (min, max);
        }

        public ValueTuple<int, int> y_position()
        {
            int min = position[0][0], max = position[0][0];

            for (int i = 0; i < 3; i++)
            {
                if (position[i + 1][0] > position[i][0])
                    max = position[i + 1][0];

                if (position[i + 1][0] < position[i][0])
                    min = position[i + 1][0];
            }

            return (min, max);
        }
    }
}
