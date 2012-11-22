using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Windows.Forms;

namespace ProjectAlphaIota
{
    public enum CheckerStatus
    {
        LOSE = -1,
        DRAW,
        WIN,
        CONTINUE
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CheckerGame : Microsoft.Xna.Framework.Game
    {
        static int rows = 6;
        static int cols = 6;
        static int TILE_SCALE = 80;
        static int TIME_DELAY = 500;
        static int MAX_FIREWORKS = 4;
        public int difficulty = 15;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CheckerBoard checkerBoard;
        ParticleManager particleManager;
        private UIScreenManager screenManager;
        Camera cam;
        SpriteFont font;
        CheckerPiece bestPiece;
        CheckerTile bestMove;
        ParticleSystem selectedParticle = null;
        Thread oThread = null;
        static Random rand = new Random();
        private Form setupForm;
        private bool setupDialogOff = true;
        private DrawDriver drawDriver;

        public enum VS_TYPE
        {
            CPU_VS_CPU,
            PLAYER_VS_CPU,
            PLAYER_VS_PLAYER
        }
        public enum GameStatus
        {
            SETUP,
            IN_PROGRESS,
            GAME_OVER
        }

        int currentTurn = 1; //Current turn 1 = black 0 = white
        public int PlayerColor = 1; //The player's color
        public VS_TYPE vs_type = VS_TYPE.CPU_VS_CPU; //Player vs CPU as opposed to CPU vs CPU
        string[] color = new string[2] { "red", "black" };
        public static int[] difficultyList = new int[4] { 5, 10, 15, 20 };
        string message = "";

        private int max_pruning = 0;
        private int min_pruning = 0;
        private int max_depth = 0;
        float timer = 0;

        public GameStatus currentGameStatus = GameStatus.SETUP;

        public CheckerGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 600;
            this.IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            checkerBoard = new CheckerBoard(6, 6, 0, 0, TILE_SCALE);
            cam = new Camera();
            cam.Pos = new Vector2(0, 0);
            particleManager = new ParticleManager();
            screenManager = new UIScreenManager();
            setupForm = new SetupForm(this);
            drawDriver = new DrawDriver();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            particleManager.LoadContent(Content, GraphicsDevice);
            screenManager.LoadContent(Content, GraphicsDevice);

            //screenManager.Add(SCREEN_STATE.IN_GAME_SCREEN, ingame_screen, true);
            drawDriver.LoadContent(GraphicsDevice, Content, particleManager);
            font = Content.Load<SpriteFont>("SpriteFont1");
            // TODO: use this.Content to load your game content here
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void HandleInput(GameTime gameTime)
        {
            if (currentGameStatus == GameStatus.SETUP)
            {

            }

            else if (currentGameStatus == GameStatus.IN_PROGRESS)
            {

                if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {

                    if ((vs_type == VS_TYPE.PLAYER_VS_CPU && currentTurn == PlayerColor) || vs_type == VS_TYPE.PLAYER_VS_PLAYER)
                    {
                        Vector2 worldPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(cam.get_transformation(GraphicsDevice)));
                        CheckerTile tile = checkerBoard.GetCheckerTile(worldPosition);

                        //Handle mouse input for the game if it is player vs player or (player vs cpu if it is the player's turn)
                        if (vs_type == VS_TYPE.PLAYER_VS_PLAYER || vs_type == VS_TYPE.PLAYER_VS_CPU && currentTurn == PlayerColor)
                        {
                            //Check that the user clicked on a tile
                            if (tile != null)
                            {
                                //User clicked on his own piece               
                                CheckerPiece checkerPiece = null;
                                if ((checkerPiece = checkerBoard.GetCheckerPiece(tile)) != null && checkerPiece.Color == currentTurn)
                                {
                                    checkerBoard.SelectedPiece = checkerPiece;
                                    if (selectedParticle != null)
                                    {
                                        selectedParticle.status = ParticleStatus.Dead;
                                        selectedParticle = null;
                                    }
                                    selectedParticle = particleManager.Spawn(particleManager.particleSystemInfoDict["fireSmall"], checkerBoard.GetCenterOfTile(checkerPiece.Row, checkerPiece.Col));
                                }

                                //If a user has selected a piece and the piece is a movable piece
                                if (checkerBoard.SelectedPiece != null && checkerBoard.MovablePieces[currentTurn].Contains(checkerBoard.SelectedPiece)
                                    && checkerBoard.MoveDict.ContainsKey(checkerBoard.SelectedPiece) && checkerBoard.MoveDict[checkerBoard.SelectedPiece].Contains(tile))
                                {
                                    //Handle the move to that location
                                    checkerBoard.HandleMove(tile.Row, tile.Col);
                                    if (selectedParticle != null)
                                    {
                                        selectedParticle.status = ParticleStatus.Dead;
                                        selectedParticle = null;
                                    }
                                    //Move to the next turn
                                    currentTurn = checkerBoard.NextTurn(currentTurn);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Allows the game to exit
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    this.Exit();



                if (currentGameStatus == GameStatus.SETUP)
                {
                    if (setupDialogOff)
                    {
                        setupDialogOff = false;
                        setupForm.ShowDialog();
                    }
                    /*
                    Renew();
                    currentGameStatus = GameStatus.IN_PROGRESS;
                     */
                }
                else if (currentGameStatus == GameStatus.IN_PROGRESS)
                {

                    CheckerStatus status = checkerBoard.GetStatus(currentTurn);
                    if (status != CheckerStatus.CONTINUE)
                        currentGameStatus = GameStatus.GAME_OVER;
                    if (status == CheckerStatus.WIN)
                    {
                        message = color[currentTurn] + " Wins!";
                        Console.WriteLine("{0} wins!", color[currentTurn]);
                    }
                    else if (status == CheckerStatus.LOSE)
                    {
                        message = String.Format("{0} wins!", color[(currentTurn + 1) % 2]);
                        Console.WriteLine(message);
                    }
                    else if (status == CheckerStatus.DRAW)
                    {
                        message = "DRAW";
                        Console.WriteLine("Draw");
                    }
                    else
                    {
                        if (vs_type == VS_TYPE.CPU_VS_CPU)
                        {
                            if (oThread == null)
                            {
                                oThread = new Thread(new ThreadStart(AI));
                                oThread.Start();
                            }
                        }
                        else if (vs_type == VS_TYPE.PLAYER_VS_CPU)
                        {
                            if (PlayerColor != currentTurn)
                            {
                                if (oThread == null)
                                {
                                    oThread = new Thread(new ThreadStart(AI));
                                    oThread.Start();

                                }                            
                            }
                        }

                    }

                    if (oThread != null)
                    {
                        if (!oThread.IsAlive)
                        {
                            oThread.Join();
                            oThread = null;
                            //Move to the next turn
                            currentTurn = checkerBoard.NextTurn(currentTurn);
                        }
                    }

                    checkerBoard.Update(gameTime, cam);
                }
                else if (currentGameStatus == GameStatus.GAME_OVER)
                {
                    timer += delta;
                    if (particleManager.particleSystems.Count() < MAX_FIREWORKS)
                    {
                        if (timer > .5)
                        {
                            timer = 0f;
                            particleManager.Spawn(particleManager.particleSystemInfoDict["firework"], new Vector2(rand.Next((int)(GraphicsDevice.Viewport.Width * .1f), (int)(GraphicsDevice.Viewport.Width * .9f)), rand.Next((int)(GraphicsDevice.Viewport.Height * .1f), (int)(GraphicsDevice.Viewport.Height * .90f))), 1f);
                        }

                    }
                }
                particleManager.Update(gameTime);
                HandleInput(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            //checkerBoard.Draw(spriteBatch, cam);
            drawDriver.DrawCheckerBoard(checkerBoard.TileBoard, checkerBoard.AllPieces, checkerBoard.SelectedPiece, checkerBoard.MovablePieces, checkerBoard.MoveDict,checkerBoard.TILE_SCALE, spriteBatch, cam);
            particleManager.Draw(spriteBatch, cam);
            if (currentGameStatus == GameStatus.SETUP)
            {
                
            }
            else if (currentGameStatus == GameStatus.IN_PROGRESS)
            {

            }
            else if (currentGameStatus == GameStatus.GAME_OVER)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), Color.Black);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        //Alpha Beta

        void AI()
        {
            bestMove = null;
            bestPiece = null;
            AlphaBetaSearch(checkerBoard, difficulty, currentTurn);
            //If it never found something b/c it was limited in depth
            if (bestMove == null && bestPiece == null)
            {
                var randomPiece = rand.Next(0, checkerBoard.MovablePieces[currentTurn].Count - 1);
                var randomMove = rand.Next(0, checkerBoard.MoveDict[checkerBoard.MovablePieces[currentTurn][randomPiece]].Count - 1);
                bestMove = checkerBoard.MoveDict[checkerBoard.MovablePieces[currentTurn][randomPiece]][randomMove];
                bestPiece = checkerBoard.MovablePieces[currentTurn][randomPiece];
            }
            checkerBoard.SelectedPiece = bestPiece;
            Console.WriteLine("Moving Piece at Row: {0}, Col: {1}", bestPiece.Row, bestPiece.Col);
            Console.WriteLine("To Row: {0}, Col: {1}", bestMove.Row, bestMove.Col);
            Console.WriteLine("Max Depth: {0}", max_depth);
            Console.WriteLine("# of Pruning in Max: {0}", max_pruning);
            Console.WriteLine("# of Pruning in Max: {0}", min_pruning);
            
            if (selectedParticle != null)
            {
                selectedParticle.status = ParticleStatus.Dead;
                selectedParticle = null;
            }
            selectedParticle = particleManager.Spawn(particleManager.particleSystemInfoDict["fireSmall"], checkerBoard.GetCenterOfTile(bestPiece.Row, bestPiece.Col));

            Thread.Sleep(TIME_DELAY);
            //Handle the move to that location
            checkerBoard.HandleMove(bestMove.Row, bestMove.Col);



            if (selectedParticle != null)
            {
                selectedParticle.status = ParticleStatus.Dead;
                selectedParticle = null;
            }

        }
        int AlphaBetaSearch(CheckerBoard currentBoard, int depth, int playerColor)
        {
            max_pruning = 0;
            min_pruning = 0;
            max_depth = 0;
            int value = MAX_VALUE(checkerBoard, 0, -999, 999, playerColor);
            return value;
        }
        int MAX_VALUE(CheckerBoard currentBoard, int depth, int alpha, int beta, int currentPlayer)
        {
            CheckerStatus status = currentBoard.GetStatus(currentPlayer);
            if (status != CheckerStatus.CONTINUE)
                return (int)status;
            max_depth = Math.Max(max_depth, depth);
            if (depth == difficulty)
                return -999;
            int v = -999;
            for (var i = 0; i < currentBoard.MovablePieces[currentPlayer].Count(); i++)
            {
                for (var j = 0; j < currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]].Count(); j++)
                {
                    //For each possible move make a new checkerboard and move it
                    
                    var newCheckerBoard = new CheckerBoard(currentBoard);
                    var selectedPiece = newCheckerBoard.GetCheckerPiece(currentBoard.MovablePieces[currentPlayer][i].Row, currentBoard.MovablePieces[currentPlayer][i].Col);
                    newCheckerBoard.SelectedPiece = selectedPiece;
                    newCheckerBoard.HandleMove(currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Row, currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Col);
                    newCheckerBoard.CheckAllAvailableMoves();
                    var nextTurn = newCheckerBoard.NextTurn(currentPlayer);

                    if (nextTurn == currentPlayer)
                    {
                        v = Math.Max(v, MAX_VALUE(newCheckerBoard, depth, alpha, beta, nextTurn));
                    }
                    else
                    {
                        v = Math.Max(v, MIN_VALUE(newCheckerBoard, depth + 1, alpha, beta, nextTurn));

                        if (v >= beta)
                        {
                            max_pruning++;
                            return v;
                        }

                        if (v > alpha)
                        {
                            alpha = v;
                            if (currentBoard == checkerBoard)
                            {
                                bestMove = currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j];
                                bestPiece = currentBoard.MovablePieces[currentPlayer][i];
                            }
                        }
                    }
                }
            }
            return v;
        }
        int MIN_VALUE(CheckerBoard currentBoard, int depth, int alpha, int beta, int currentPlayer)
        {

            CheckerStatus status = currentBoard.GetStatus(currentPlayer);

            if (status != CheckerStatus.CONTINUE)
                return (int)status;

            max_depth = Math.Max(max_depth, depth);
            if (depth == difficulty)
                return 999;
            int v = 999;
            //For each movable piece
            for (var i = 0; i < currentBoard.MovablePieces[currentPlayer].Count(); i++)
            {
                //for each possible positions that the piece can go
                for (var j = 0; j < currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]].Count(); j++)
                {
                    //For each possible move make a new checkerboard and move it
                    CheckerBoard newCheckerBoard = new CheckerBoard(currentBoard);
                    CheckerPiece SelectedPiece = newCheckerBoard.GetCheckerPiece(currentBoard.MovablePieces[currentPlayer][i].Row, currentBoard.MovablePieces[currentPlayer][i].Col);
                    newCheckerBoard.SelectedPiece = SelectedPiece;
                    newCheckerBoard.HandleMove(currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Row, currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Col);
                    newCheckerBoard.CheckAllAvailableMoves();
                    int nextTurn = newCheckerBoard.NextTurn(currentPlayer);

                    if (nextTurn == currentPlayer)
                        v = Math.Min(v, MIN_VALUE(newCheckerBoard, depth, alpha, beta, nextTurn));
                    else
                    {
                        v = Math.Min(v, MAX_VALUE(newCheckerBoard, depth + 1, alpha, beta, nextTurn));
                        //pruning
                        if (v <= alpha)
                        {
                            min_pruning++;
                            return v;
                        }

                        if (v < beta)
                        {
                            beta = v;
                            if (currentBoard == checkerBoard)
                            {
                                bestMove = currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j];
                                bestPiece = currentBoard.MovablePieces[currentPlayer][i];
                            }
                        }
                    }
                }
            }
            return v;
        }

        public void Renew()
        {
            message = "";
            currentTurn = 1;
            checkerBoard.Reset();

            if (selectedParticle != null)
            {
                selectedParticle.status = ParticleStatus.Dead;
            }
        }


    }
}
