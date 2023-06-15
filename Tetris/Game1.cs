using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;

namespace Tetris
{
    public class Game1 : Game
    {
        //private const int GridSize = 10; // Number of rows and columns in the grid
        //private const int CellSize = 40; // Size of each cell in pixels

        //Texture2D ballTexture;
        //int score = 0;
        //bool control = false;

        SpriteFont spriteFont;

        //MouseState mouseState;

        Texture2D background;
        Texture2D block;

        GameBoard gameboard;
        Tetromino tetromino;

        KeyboardState keyboardState;
        int spritePosition = 0;
        int speed = 0;

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 549;
            _graphics.PreferredBackBufferHeight = 672;
            _graphics.ApplyChanges();

            gameboard = new(22, 11);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //ballTexture = Content.Load<Texture2D>("ball");
            spriteFont = Content.Load<SpriteFont>("galleryFont");

            background = Content.Load<Texture2D>("726c6346330007.5607cdd1439a0");
            block = Content.Load<Texture2D>("block");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (speed == 0)
            {
                tetromino = new Tetromino();

                TetrominoMovement.TetrominoInitialize(gameboard.grid, tetromino);
            }

            keyboardState = Keyboard.GetState();

            speed++;

            if (keyboardState.IsKeyDown(Keys.Left) && speed % 4 == 0 && tetromino.x_position().Item1 != 0)
            {
                TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, -1);
            }

            else if (keyboardState.IsKeyDown(Keys.Right) && speed % 4 == 0 && tetromino.x_position().Item2 != 10)
            {
                TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, 1);
            }

            if (keyboardState.IsKeyDown(Keys.Up) && speed % 4 == 0)
            {
                TetrominoMovement.TetrominoRotate(gameboard.grid, tetromino);
            }

            if (keyboardState.IsKeyDown(Keys.Down) && speed % 4 == 0 && tetromino.y_position().Item2 != 21)
            {
                TetrominoMovement.TetrominoDrop(gameboard.grid, tetromino);
            }

            if (speed % 15 == 0 && tetromino.y_position().Item2 != 21)
            {
                TetrominoMovement.TetrominoDrop(gameboard.grid, tetromino);
            }

            for (int i = 0; i < 4; i++)
            {
                if (tetromino.y_position().Item2 == 21 || gameboard.grid[tetromino.position[i][0] + 1, tetromino.position[i][1]] < 0)
                {
                    TetrominoMovement.TetrominoLand(gameboard.grid, tetromino);

                    tetromino = new Tetromino();
                    TetrominoMovement.TetrominoInitialize(gameboard.grid, tetromino);

                    break;
                }
            }

            //mouseState = Mouse.GetState();

            //float MouseTargetDist = Vector2.Distance(new Vector2(200, 200), mouseState.Position.ToVector2());

            //if (mouseState.LeftButton == ButtonState.Pressed && control == false)
            //{
            //    score++;
            //    control = true;
            //}

            //if (mouseState.LeftButton == ButtonState.Released)
            //{
            //    control = false;
            //}

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            //for (int row = 0; row < GridSize; row++)
            //{
            //    for (int col = 0; col < GridSize; col++)
            //    {
            //        // Calculate the position of each cell
            //        Vector2 cellPosition = new Vector2(col * CellSize, row * CellSize);

            //        // Determine the color of each cell based on the value in the grid
            //        Color cellColor = (grid[row, col] == 1) ? Color.White : Color.Black;

            //        // Draw a rectangle for each cell
            //        _spriteBatch.Draw(
            //            texture: ballTexture, // You can use a texture if you want to display a specific image for each cell
            //            destinationRectangle: new Rectangle((int)cellPosition.X, (int)cellPosition.Y, CellSize, CellSize),
            //            color: cellColor
            //        );
            //    }
            //}

            //_spriteBatch.DrawString(spriteFont, text: score.ToString(), new Vector2(200, 200), color: Color.White);

            _spriteBatch.Draw(
                        texture: background,
                        position: new Vector2(0, 0),
                        color: Color.White
                    );

            for (int i = 0; i < 22; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (gameboard.grid[i, j] == 1)
                        _spriteBatch.Draw(block, new Vector2(31 + 28 * j, 25 + 28 * i), tetromino.color);

                    if (gameboard.grid[i, j] < 0)
                        _spriteBatch.Draw(block, new Vector2(31 + 28 * j, 25 + 28 * i), tetromino.colorSelection(-gameboard.grid[i, j] - 1));

                    _spriteBatch.DrawString(spriteFont, text: gameboard.grid[i, j].ToString(), new Vector2(31 + 28 * j, 25 + 28 * i), color: Color.White);
                }
            }

            for (int i = 0; i < 4; i++)
                _spriteBatch.DrawString(spriteFont, text: tetromino.position[i][0].ToString(), new Vector2(400 + 20 * i, 120), color: Color.White);
            for (int i = 0; i < 4; i++)
                _spriteBatch.DrawString(spriteFont, text: tetromino.position[i][1].ToString(), new Vector2(400 + 20 * i, 160), color: Color.White);

            //_spriteBatch.DrawString(spriteFont, text: tetromino.x_position().Item1.ToString(), new Vector2(400, 550), color: Color.White);
            //_spriteBatch.DrawString(spriteFont, text: tetromino.x_position().Item2.ToString(), new Vector2(450, 550), color: Color.White);

            //_spriteBatch.DrawString(spriteFont, text: tetromino.y_position().Item1.ToString(), new Vector2(400, 600), color: Color.White);
            //_spriteBatch.DrawString(spriteFont, text: tetromino.y_position().Item2.ToString(), new Vector2(450, 600), color: Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    //public static class SpriteBatchExtensions
    //{
    //    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness = 1)
    //    {
    //        Vector2 delta = end - start;
    //        float angle = (float)Math.Atan2(delta.Y, delta.X);
    //        spriteBatch.Draw(Texture2D, start, null, color, angle, Vector2.Zero, new Vector2(delta.Length(), thickness), SpriteEffects.None, 0);
    //    }
    //}
}
