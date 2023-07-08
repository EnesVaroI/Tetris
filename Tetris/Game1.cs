using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Threading;
using System.Timers;

namespace Tetris
{
    public class Game1 : Game
    {
        SpriteFont spriteFont;

        Texture2D background;
        Texture2D block;
        Texture2D frame;

        GameBoard gameboard;
        Tetromino tetromino;
        Tetromino ghostTetromino;
        Tetromino nextTetromino;

        KeyboardState keyboardState;
        MouseState mouseState;

        bool isLanded = false;
        bool landing = false;
        DateTime timeOfLanding;

        int speed = 0;
        bool mouseControl = false;
        bool keyboardControl = false;

        int linesCleared = 0;
        int score = 0;
        int level = 1;

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

            spriteFont = Content.Load<SpriteFont>("galleryFont");

            background = Content.Load<Texture2D>("726c6346330007.5607cdd1439a0");
            block = Content.Load<Texture2D>("block");
            frame = Content.Load<Texture2D>("frame");
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

                ghostTetromino = new Tetromino(tetromino);

                nextTetromino = new Tetromino(new Random().Next(0, 7));
            }

            isLanded = tetromino.y_position().Item2 == 21 || Enumerable.Range(0, 4).Any(i => gameboard.grid[tetromino.position[i][0] + 1, tetromino.position[i][1]] < 0);

            int[][] temp_ghostTetromino;
            TetrominoMovement.GhostTetrominoLocate(gameboard.grid, tetromino, out temp_ghostTetromino);
            ghostTetromino.position = temp_ghostTetromino;

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            speed++;

            if (mouseState.LeftButton == ButtonState.Pressed && speed % 4 == 0 && mouseControl == false)
            {
                TetrominoMovement.TetrominoInstaPlace(gameboard.grid, tetromino, ghostTetromino.position);
                mouseControl = true;
                goto EndTurn;
            }

            if (mouseState.LeftButton == ButtonState.Released)
                mouseControl = false;

            if (keyboardState.IsKeyDown(Keys.Left) && speed % 4 == 0 && tetromino.x_position().Item1 != 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (gameboard.grid[tetromino.position[i][0], tetromino.position[i][1] - 1] < 0)
                        break;
                    
                    if (i == 3)
                        TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, -1);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Right) && speed % 4 == 0 && tetromino.x_position().Item2 != 10)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (gameboard.grid[tetromino.position[i][0], tetromino.position[i][1] + 1] < 0)
                         break;

                    if (i == 3)
                        TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, 1);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Up) && speed % 4 == 0 && keyboardControl == false)
            {
                TetrominoMovement.TetrominoRotate(gameboard.grid, tetromino);
                keyboardControl = true;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Up))
                keyboardControl = false;

            if (keyboardState.IsKeyDown(Keys.Down) && speed % 4 == 0 && !isLanded)
            {
                TetrominoMovement.TetrominoDrop(gameboard.grid, tetromino);
            }

            if (speed % (21 - Math.Min(20, level)) == 0 && !isLanded)
            {
                TetrominoMovement.TetrominoDrop(gameboard.grid, tetromino);
            }

            //if (mouseState.Position.X < tetromino.length)
            //    TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, -1);

            //if (mouseState.Position.X > tetromino.length)
            //    TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, 1);

            if (isLanded && !landing)
            {
                timeOfLanding = DateTime.Now.AddSeconds(1);
                landing = true;
            }

            EndTurn:
            while (isLanded && DateTime.Now >= timeOfLanding)
            {
                TetrominoMovement.TetrominoLand(gameboard.grid, tetromino);

                int a = 0;

                for (int j = 21; j >= 0; j--)
                {
                    if (gameboard.IsRowFull(j))
                    {
                        gameboard.ClearRow(j);
                        j++;
                        
                        linesCleared++;

                        if (linesCleared >= 10 * level + level * (level - 1) / 2)
                            level++;

                        a++;
                    }
                }

                if (a != 0)
                    score += 20 * (a * a + 1) * level;

                tetromino = new Tetromino(nextTetromino.getPieceType());
                TetrominoMovement.TetrominoInitialize(gameboard.grid, tetromino);

                ghostTetromino = new Tetromino(tetromino);

                nextTetromino = new Tetromino(new Random().Next(0, 7));

                landing = false;

                break;
            }

            base.Update(gameTime);
        }
        float timer = 0.0f;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                        texture: background,
                        position: new Vector2(0, 0),
                        color: Color.White
                    );

            for (int i = 0; i < 22; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (gameboard.grid[i, j] == 1 && !isLanded)
                        _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), tetromino.color);


                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds; // Increment the timer
                    float alpha = (float)Math.Abs(Math.Sin(timer)) * 0.5f + 0.5f;
                    Color color = new Color(0.6f, 0.7f, 0.1f, alpha);
                    if (gameboard.grid[i, j] == 1 && isLanded)
                        _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), color);

                    if (gameboard.grid[i, j] < 0)
                        _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), tetromino.colorSelection(-gameboard.grid[i, j] - 1));
                }
            }

            for (int i = 0; i < nextTetromino.length; i++)
            {
                for (int j = 0; j < nextTetromino.length; j++)
                {
                    if (nextTetromino.length == 2 && nextTetromino.piece[i, j] == 1)
                        _spriteBatch.Draw(block, new Vector2(420 + 28 * j, 149 + 28 * i), nextTetromino.color);

                    else if (nextTetromino.length == 3 && nextTetromino.piece[i, j] == 1)
                        _spriteBatch.Draw(block, new Vector2(392 + 28 * j, 149 + 28 * i), nextTetromino.color);

                    else if (nextTetromino.length == 4 && nextTetromino.piece[i, j] == 1)
                        _spriteBatch.Draw(block, new Vector2(392 + 28 * j, 121 + 28 * i), nextTetromino.color);
                }
            }

            foreach (var position in TetrominoMovement.GhostTetrominoDisplay(ghostTetromino.position))
                _spriteBatch.Draw(frame, position, ghostTetromino.color);

            _spriteBatch.DrawString(spriteFont, text: "Lines: " + linesCleared.ToString(), new Vector2(370, 340), color: Color.White);
            _spriteBatch.DrawString(spriteFont, text: "Level: " + level.ToString(), new Vector2(370, 380), color: Color.White);
            _spriteBatch.DrawString(spriteFont, text: "Score: " + score.ToString(), new Vector2(370, 420), color: Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
