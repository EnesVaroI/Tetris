using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using static Tetris.Extensions;
using static Tetris.Tetromino;
using static Tetris.TetrominoMovement;
using static Tetris.ScoreRepository;
using static Tetris.TetrisDataCollector;
using static Tetris.PythonInterop;

namespace Tetris
{
    public class Game1 : Game
    {
        SpriteFont spriteFont;

        Texture2D background;
        Texture2D leaderboard;
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
        int temp_level = 1;
        string playerName = "";

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        enum GameState
        {
            Menu,
            Playing,
            Paused,
            Lost
        }
        
        void Quit(int new_level = 1)
        {
            gameboard.ClearBoard();

            landing = false;
            heldTetromino = null;
            linesCleared = 0;
            score = 0;
            level = new_level;
            speed = 0;
            timer = 0.0f;
            isPieceSwapped = false;
            playerName = String.Empty;
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

            InitializeDatabase();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("galleryFont");

            background = Content.Load<Texture2D>("726c6346330007.5607cdd1439a0");
            leaderboard = Content.Load<Texture2D>("leaderboard");
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
                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButtonType.LeftButton))
                    gameState = GameState.Playing;

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButtonType.LeftButton))
                    level = level % 20 + 1;
            }

            if (gameState == GameState.Paused)
            {
                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButtonType.LeftButton))
                    gameState = GameState.Playing;

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButtonType.LeftButton))
                {
                    gameState = GameState.Menu;
                    Quit();
                }
            }

            EndGame:
            if (gameState == GameState.Lost)
            {
                if (keyboardState.IsClick(Keys.Back) && playerName.Length > 0)
                    playerName = playerName.Substring(0, playerName.Length - 1);

                var keys = Enum.GetValues(typeof(Keys)).Cast<Keys>();

                foreach (var key in keys)
                    if (playerName.Length < 8 && keyboardState.IsClick(key))
                        playerName += keyboardState.GetTypedChar(key);

                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButtonType.LeftButton))
                {
                    if (playerName != string.Empty)
                        SaveScore(playerName, score);

                    gameState = GameState.Playing;
                    Quit(level);
                }

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButtonType.LeftButton))
                    level = level % 20 + 1;
            }

            if (gameState == GameState.Playing)
            {
                if (speed == 0)
                {
                    tetromino = new Tetromino();

                    TetrominoInitialize(gameboard.grid, tetromino);

                    ghostTetromino = new Tetromino(tetromino);

                    nextTetromino = new Tetromino((Shape)new Random().Next(0, 7));
                }

                int[][] temp_ghostTetromino;
                GhostTetrominoLocate(gameboard.grid, tetromino, out temp_ghostTetromino);
                ghostTetromino.position = temp_ghostTetromino;

                speed++;

                if (mouseState.IsInRange((22, 335), (24, 651)) && mouseState.IsClick(MouseButtonType.LeftButton) || keyboardState.IsClick(Keys.Space))
                {
                    int height = CalculateHeight(gameboard.grid, (mouseState.X - 25) / 28);
                    int[] groundState = CalculateGroundState(gameboard.grid, (mouseState.X - 25) / 28);

                    //This statement has been commented out to prevent performance crashes.
                    //StoreTetrisData(height, tetromino.shape, groundState, tetromino.rotation);

                    score += level * (ghostTetromino.position[0][0] - tetromino.position[0][0]);
                    TetrominoInstaPlace(gameboard.grid, tetromino, ghostTetromino.position);

                    goto EndTurn;
                }

                if (mouseState.IsInRange((22, 335), (24, 651))/* && !mouseState.IsSteady()*/)
                {
                    int center = 44 + 14 * tetromino.x_position().Min + 14 * tetromino.x_position().Max;
                    int direction = Math.Abs(mouseState.X - center) >= 14 ? Math.Sign(mouseState.X - center) : 0;
                    if (direction < 0 && !tetromino.isLeftmost(gameboard.grid) || direction > 0 && !tetromino.isRightmost(gameboard.grid))
                        TetrominoMove(gameboard.grid, tetromino, direction);

                    int height = CalculateHeight(gameboard.grid, (mouseState.X - 25) / 28);
                    int[] groundState = CalculateGroundState(gameboard.grid, (mouseState.X - 25) / 28);

                    if (speed % 160 == 0)
                    {
                        try
                        {
                            int rotation = InferRotation(height, (int)tetromino.shape, groundState[0], groundState[1]);
                            int turn = (rotation - tetromino.rotation + 4) % 4;

                            for (int i = 0; i < turn; i++)
                                TetrominoRotate(gameboard.grid, tetromino, RotationDirection.Clockwise);
                        }
                        catch (ZeroAreaException) { }
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Left) && speed % 4 == 0 && !tetromino.isLeftmost(gameboard.grid))
                    TetrominoMove(gameboard.grid, tetromino, -1);

                if (keyboardState.IsKeyDown(Keys.Right) && speed % 4 == 0 && !tetromino.isRightmost(gameboard.grid))
                    TetrominoMove(gameboard.grid, tetromino, 1);

                if (keyboardState.IsClick(Keys.Up))
                    TetrominoRotate(gameboard.grid, tetromino, RotationDirection.Clockwise);

                if (keyboardState.IsClick(Keys.Z))
                    TetrominoRotate(gameboard.grid, tetromino, RotationDirection.CounterClockwise);

                if (keyboardState.IsKeyDown(Keys.Down) && speed % 4 == 0 && !tetromino.isLanded(gameboard.grid))
                {
                    TetrominoDrop(gameboard.grid, tetromino);
                    score += level;
                }

                if (speed % (21 - Math.Min(20, level)) == 0 && !tetromino.isLanded(gameboard.grid))
                    TetrominoDrop(gameboard.grid, tetromino);

                if ((keyboardState.IsKeyDown(Keys.C) || mouseState.IsInRange((363, 531), (213, 326)) && mouseState.IsClick(MouseButtonType.LeftButton)
                    || mouseState.IsInRange((22, 335), (24, 651)) && mouseState.IsClick(MouseButtonType.RightButton)) && !isPieceSwapped)
                {
                    Enumerable.Range(0, 4).ToList().ForEach(i => gameboard[tetromino.position[i][0], tetromino.position[i][1]] = 0);

                    if (heldTetromino == null)
                        (heldTetromino, tetromino, nextTetromino) = (tetromino, new Tetromino(nextTetromino.shape), new Tetromino((Shape)new Random().Next(0, 7)));

                    else
                        (tetromino, heldTetromino) = (heldTetromino, tetromino);

                    TetrominoInitialize(gameboard.grid, tetromino);
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
                    TetrominoLand(gameboard.grid, tetromino);

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

                    tetromino = new Tetromino(nextTetromino.shape);

                    try
                    {
                        TetrominoInitialize(gameboard.grid, tetromino);
                    }
                    catch (GameOverException)
                    {
                        gameState = GameState.Lost;
                        temp_level = level;
                        goto EndGame;
                    }

                    ghostTetromino = new Tetromino(tetromino);

                    nextTetromino = new Tetromino((Shape)new Random().Next(0, 7));

                    isPieceSwapped = false;

                    landing = false;

                    break;
                }

                if (mouseState.IsInRange((363, 523), (541, 578)) && mouseState.IsClick(MouseButtonType.LeftButton))
                    gameState = GameState.Paused;

                if (mouseState.IsInRange((363, 523), (598, 635)) && mouseState.IsClick(MouseButtonType.LeftButton))
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
                _spriteBatch.DrawString(spriteFont, text: "Play", new Vector2(413, 542), color: Color.White);
                _spriteBatch.DrawString(spriteFont, text: "Level: " + level.ToString(), new Vector2(405 - 12 * level.ToString().Length, 599), color: Color.White);
            }

            else
            {
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
                            _spriteBatch.Draw(block, new Vector2(25 + 28 * j, 31 + 28 * i), tetromino.colorSelection((Shape)(-gameboard.grid[i, j] - 1)));
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

                foreach (var position in GhostTetrominoDisplay(ghostTetromino.position))
                    _spriteBatch.Draw(frame, position, ghostTetromino.color);

                if (gameState == GameState.Paused)
                {
                    _spriteBatch.DrawString(spriteFont, text: "Resume", new Vector2(386, 542), color: Color.White);
                    _spriteBatch.DrawString(spriteFont, text: "Quit", new Vector2(412, 599), color: Color.White);

                    _spriteBatch.DrawString(spriteFont, text: level.ToString(), new Vector2(374, 472), color: Color.White);
                }

                if (gameState == GameState.Lost)
                {
                    _spriteBatch.Draw(leaderboard, new Vector2(53, 143), Color.White);

                    _spriteBatch.DrawString()(spriteFont, "GAME OVER", new Vector2(98, 147), Color.White, 0.85f);
                    _spriteBatch.DrawString()(spriteFont, "HIGH SCORES", new Vector2(108, 187), Color.White, 0.65f);

                    for (int i = 0; i < GetTopScores().Count; i++)
                    {
                        _spriteBatch.DrawString()(spriteFont, GetTopScores()[i].Name, new Vector2(80, (int)(212 + 20.5 * i)), Color.White, 0.5f);
                        _spriteBatch.DrawString()(spriteFont, GetTopScores()[i].Score.ToString(), new Vector2(278 - 9 * GetTopScores()[i].Score.ToString().Length, (int)(212 + 20.5 * i)), Color.White, 0.5f);
                    }

                    _spriteBatch.DrawString()(spriteFont, "Please enter your name:", new Vector2(79, 472), Color.White, 0.45f);
                    _spriteBatch.DrawString()(spriteFont, playerName, new Vector2(82, 493), Color.White, 0.75f);

                    _spriteBatch.DrawString(spriteFont, text: "Play", new Vector2(413, 542), color: Color.White);
                    _spriteBatch.DrawString(spriteFont, text: "Level: " + level.ToString(), new Vector2(405 - 12 * level.ToString().Length, 599), color: Color.White);

                    _spriteBatch.DrawString(spriteFont, text: temp_level.ToString(), new Vector2(374, 472), color: Color.White);
                }

                else if (gameState == GameState.Playing)
                {
                    _spriteBatch.DrawString(spriteFont, text: "Pause", new Vector2(401, 542), color: Color.White);
                    _spriteBatch.DrawString(spriteFont, text: "Quit", new Vector2(412, 599), color: Color.White);

                    _spriteBatch.DrawString(spriteFont, text: level.ToString(), new Vector2(374, 472), color: Color.White);
                }

                //_spriteBatch.DrawString(spriteFont, text: "Lines: " + linesCleared.ToString(), new Vector2(370, 540), color: Color.White);
                _spriteBatch.DrawString(spriteFont, text: score.ToString(), new Vector2(374, 380), color: Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}