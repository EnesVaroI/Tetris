using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using static Tetris.Extensions;
using static Tetris.TetrominoMovement;

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
        Tetromino heldTetromino;

        GameState gameState;
        KeyboardState keyboardState;
        MouseState mouseState;

        bool landing = false;
        DateTime timeOfLanding;

        int speed = 0;
        float timer = 0.0f;
        bool isPieceSwapped = false;

        int linesCleared = 0;
        int score = 0;
        int level = 1;

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        enum GameState
        {
            Menu,
            Paused,
            Playing
        }

        void Quit()
        {
            gameboard.ClearBoard();

            landing = false;
            heldTetromino = null;
            linesCleared = 0;
            score = 0;
            level = 1;
            speed = 0;
            timer = 0.0f;
            isPieceSwapped = false;
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 549;
            _graphics.PreferredBackBufferHeight = 672;
            _graphics.ApplyChanges();

            gameState = GameState.Menu;
            gameboard = new(22, 11);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("galleryFont");

            background = Content.Load<Texture2D>("726c6346330007.5607cdd1439a0");
            block = Content.Load<Texture2D>("block");
            frame = Content.Load<Texture2D>("frame");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (gameState == GameState.Menu)
            {
                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButton.LeftButton))
                    gameState = GameState.Playing;

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButton.LeftButton))
                    level = level % 20 + 1;
            }

            if (gameState == GameState.Paused)
            {
                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButton.LeftButton))
                    gameState = GameState.Playing;

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButton.LeftButton))
                {
                    gameState = GameState.Menu;
                    Quit();
                }
            }

            if (gameState == GameState.Playing)
            {
                if (speed == 0)
                {
                    tetromino = new Tetromino();

                    TetrominoMovement.TetrominoInitialize(gameboard.grid, tetromino);

                    ghostTetromino = new Tetromino(tetromino);

                    nextTetromino = new Tetromino(new Random().Next(0, 7));
                }

                int[][] temp_ghostTetromino;
                TetrominoMovement.GhostTetrominoLocate(gameboard.grid, tetromino, out temp_ghostTetromino);
                ghostTetromino.position = temp_ghostTetromino;

                speed++;

                if (mouseState.IsInRange((22, 335), (24, 651)) && mouseState.IsClick(MouseButton.LeftButton) || keyboardState.IsClick(Keys.Space))
                {
                    score += level * (ghostTetromino.position[0][0] - tetromino.position[0][0]);
                    TetrominoMovement.TetrominoInstaPlace(gameboard.grid, tetromino, ghostTetromino.position);
                    goto EndTurn;
                }

                if (mouseState.IsInRange((22, 335), (24, 651)) && !mouseState.IsSteady())
                {
                    int center = 44 + 14 * tetromino.x_position().Item1 + 14 * tetromino.x_position().Item2;
                    int direction = Math.Abs(mouseState.X - center) >= 14 ? Math.Sign(mouseState.X - center) : 0;
                    if (direction < 0 && !tetromino.isLeftmost(gameboard.grid) || direction > 0 && !tetromino.isRightmost(gameboard.grid))
                        TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, direction);
                }

                if (keyboardState.IsKeyDown(Keys.Left) && speed % 4 == 0 && !tetromino.isLeftmost(gameboard.grid))
                    TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, -1);

                if (keyboardState.IsKeyDown(Keys.Right) && speed % 4 == 0 && !tetromino.isRightmost(gameboard.grid))
                    TetrominoMovement.TetrominoMove(gameboard.grid, tetromino, 1);

                if (keyboardState.IsClick(Keys.Up))
                    TetrominoMovement.TetrominoRotate(gameboard.grid, tetromino, RotationDirection.Clockwise);

                if (keyboardState.IsClick(Keys.Z))
                    TetrominoMovement.TetrominoRotate(gameboard.grid, tetromino, RotationDirection.CounterClockwise);

                if (keyboardState.IsKeyDown(Keys.Down) && speed % 4 == 0 && !tetromino.isLanded(gameboard.grid))
                {
                    TetrominoMovement.TetrominoDrop(gameboard.grid, tetromino);
                    score += level;
                }

                if (speed % (21 - Math.Min(20, level)) == 0 && !tetromino.isLanded(gameboard.grid))
                    TetrominoMovement.TetrominoDrop(gameboard.grid, tetromino);

                if ((keyboardState.IsKeyDown(Keys.C) || mouseState.IsInRange((363, 531), (213, 326)) && mouseState.IsClick(MouseButton.LeftButton)
                    || mouseState.IsInRange((22, 335), (24, 651)) && mouseState.IsClick(MouseButton.RightButton)) && !isPieceSwapped)
                {
                    Enumerable.Range(0, 4).ToList().ForEach(i => gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 0);

                    if (heldTetromino == null)
                        (heldTetromino, tetromino, nextTetromino) = (tetromino, new Tetromino(nextTetromino.type), new Tetromino(new Random().Next(0, 7)));

                    else
                        (tetromino, heldTetromino) = (heldTetromino, tetromino);

                    TetrominoMovement.TetrominoInitialize(gameboard.grid, tetromino);
                    ghostTetromino = new Tetromino(tetromino);
                    isPieceSwapped = true;
                }

                EndTurn:
                if (tetromino.isLanded(gameboard.grid) && !landing)
                {
                    timeOfLanding = DateTime.Now.AddSeconds(1);
                    landing = true;
                }

                while (tetromino.isLanded(gameboard.grid) && DateTime.Now >= timeOfLanding)
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

                    tetromino = new Tetromino(nextTetromino.type);
                    TetrominoMovement.TetrominoInitialize(gameboard.grid, tetromino);

                    ghostTetromino = new Tetromino(tetromino);

                    nextTetromino = new Tetromino(new Random().Next(0, 7));

                    isPieceSwapped = false;

                    landing = false;

                    break;
                }

                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButton.LeftButton))
                    gameState = GameState.Paused;

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButton.LeftButton))
                {
                    gameState = GameState.Menu;
                    Quit();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: background,
                position: new Vector2(0, 0),
                color: Color.White
            );

            if (gameState == GameState.Menu)
            {
                _spriteBatch.DrawString(spriteFont, text: "Play", new Vector2(400, 542), color: Color.White);
                _spriteBatch.DrawString(spriteFont, text: "Level: " + level.ToString(), new Vector2(400, 599), color: Color.White);
            }

            else
            {
                if (gameState == GameState.Paused)
                {
                    _spriteBatch.DrawString(spriteFont, text: "Resume", new Vector2(400, 542), color: Color.White);
                    _spriteBatch.DrawString(spriteFont, text: "Quit", new Vector2(400, 599), color: Color.White);
                }

                else if (gameState == GameState.Playing)
                {
                    _spriteBatch.DrawString(spriteFont, text: "Pause", new Vector2(400, 542), color: Color.White);
                    _spriteBatch.DrawString(spriteFont, text: "Quit", new Vector2(400, 599), color: Color.White);
                }

                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = 0; i < 22; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        if (gameboard.grid[i, j] == 1 && !tetromino.isLanded(gameboard.grid))
                            _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), tetromino.color);

                        if (gameboard.grid[i, j] == 1 && tetromino.isLanded(gameboard.grid))
                            _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), tetromino.color * ((MathF.Sin(timer * MathF.PI * 2) + 1) / 2));

                        if (gameboard.grid[i, j] < 0)
                            _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), tetromino.colorSelection(-gameboard.grid[i, j] - 1));
                    }
                }

                for (int i = 0; i < nextTetromino.length; i++)
                {
                    for (int j = 0; j < nextTetromino.length; j++)
                    {
                        if (nextTetromino.length == 2 && nextTetromino.piece[i, j] == 1)
                            _spriteBatch.Draw(block, new Vector2(420 + 28 * j, 81 + 28 * i), nextTetromino.color);

                        else if (nextTetromino.length == 3 && nextTetromino.piece[i, j] == 1)
                            _spriteBatch.Draw(block, new Vector2(392 + 28 * j, 81 + 28 * i), nextTetromino.color);

                        else if (nextTetromino.length == 4 && nextTetromino.piece[i, j] == 1)
                            _spriteBatch.Draw(block, new Vector2(392 + 28 * j, 53 + 28 * i), nextTetromino.color);
                    }
                }

                for (int i = 0; heldTetromino != null && i < heldTetromino.length; i++)
                {
                    for (int j = 0; j < heldTetromino.length; j++)
                    {
                        if (heldTetromino.length == 2 && heldTetromino.piece[i, j] == 1)
                            _spriteBatch.Draw(block, new Vector2(420 + 28 * j, 243 + 28 * i), heldTetromino.color);

                        else if (heldTetromino.length == 3 && heldTetromino.piece[i, j] == 1)
                            _spriteBatch.Draw(block, new Vector2(392 + 28 * j, 243 + 28 * i), heldTetromino.color);

                        else if (heldTetromino.length == 4 && heldTetromino.piece[i, j] == 1)
                            _spriteBatch.Draw(block, new Vector2(392 + 28 * j, 215 + 28 * i), heldTetromino.color);
                    }
                }

                foreach (var position in TetrominoMovement.GhostTetrominoDisplay(ghostTetromino.position))
                    _spriteBatch.Draw(frame, position, ghostTetromino.color);

                //_spriteBatch.DrawString(spriteFont, text: "Lines: " + linesCleared.ToString(), new Vector2(370, 540), color: Color.White);
                _spriteBatch.DrawString(spriteFont, text: level.ToString(), new Vector2(374, 472), color: Color.White);
                _spriteBatch.DrawString(spriteFont, text: score.ToString(), new Vector2(374, 380), color: Color.White);
            }

            _spriteBatch.DrawString(spriteFont, text: mouseState.X.ToString() + ", " + mouseState.Y.ToString() + "    " + mouseState.IsSteady().ToString(), new Vector2(100, 100), color: Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}