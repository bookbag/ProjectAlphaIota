using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Windows.Forms;

namespace ProjectAlphaIota
{
    public enum CheckerStatus
    {
        Lose = -999,
        Continue = 0,
        Win = 999     
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CheckerGame : Game
    {
        public int Rows = 6;
        public int Cols = 6;
        private const int TileScale = 80;
        private const int TimeDelay = 500;
        private const int MaxFireworks = 4;
        private const int WINDOW_WIDTH = 640;
        private const int WINDOW_HEIGHT = 480;

        readonly GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;
        CheckerBoard _checkerBoard;
        ParticleManager _particleManager;
        private UIScreenManager _screenManager;

        Camera _cam;
        SpriteFont _font;
        CheckerPiece _bestPiece;
        CheckerTile _bestMove;
        ParticleSystem _selectedParticle;
        Thread _oThread;

        static readonly Random Rand = new Random();
        private Form _setupForm;
        private bool _setupDialogOff = true;
        private DrawDriver _drawDriver;

        private float TimeElapsed = 0;

        private Lightning bolt;
        private Effect Colorize, GaussianBlur;

        public enum VsType
        {
            CpuVsCpu,
            PlayerVsCpu,
            PlayerVsPlayer
        }
        public enum GameStatus
        {
            Setup,
            InProgress,
            GameOver
        }

        int _currentTurn = 1; //Current turn 1 = black 0 = white
        public int PlayerColor = 1; //The player's color
        public VsType CurrentVsType = VsType.CpuVsCpu; //Player vs CPU as opposed to CPU vs CPU
        readonly string[] _color = new[] { "Red", "Black" };
        public static int[] DifficultyList = new[] { 5, 10, 15, 20 };
        string _message = "";
        public int Difficulty = 14;
        private int _maxPruning;
        private int _minPruning;
        private int _maxDepth;
        private int _nodeGeneration;
        float _timer;
        RenderTarget2D _sceneRenderTarget, _renderTargetHalved, _renderTargetQuatered, _renderTargetEighted, _origTarget;
        public GameStatus CurrentGameStatus = GameStatus.Setup;

        public CheckerGame()
        {            
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _maxPruning = 0;
            _minPruning = 0;
            _maxDepth = 0;
            _timer = 0;
            _nodeGeneration = 0;
            _checkerBoard = new CheckerBoard(Rows, Cols);
            _cam = new Camera {Pos = new Vector2(0, 0)};
            _particleManager = new ParticleManager();
            _screenManager = new UIScreenManager();
            _setupForm = new SetupForm(this);
            _drawDriver = new DrawDriver();
            base.Initialize();
        }

        public void ShowNewGameDialog()
        {
            if (_setupForm.Visible == false)
                _setupForm.ShowDialog();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _particleManager.LoadContent(Content, GraphicsDevice);
            _screenManager.LoadContent(Content, GraphicsDevice);
            var gameScreen = new UIGameScreen();
            
            _screenManager.Add(SCREEN_STATE.InGameScreen, gameScreen, true);
            gameScreen.AddItem(new Rectangle(GraphicsDevice.Viewport.Width - 149, GraphicsDevice.Viewport.Height - 40, 148, 30), "New Game", null, ShowNewGameDialog);
            
            //screenManager.Add(SCREEN_STATE.IN_GAME_SCREEN, ingame_screen, true);
            _drawDriver.LoadContent(GraphicsDevice, Content, _particleManager);
            _font = Content.Load<SpriteFont>("SpriteFont1");
            Colorize = Content.Load<Effect>(@"Effects/Colorize");
            GaussianBlur = Content.Load<Effect>(@"Effects//GaussianBlur");

            bolt = new Lightning(new Vector3(100, 200, 0), new Vector3(600, 400, 0));
            bolt.LoadContent(Content,GraphicsDevice);
            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            SurfaceFormat format = pp.BackBufferFormat;

            _sceneRenderTarget = new RenderTarget2D(GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            _renderTargetHalved = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, false, format, DepthFormat.None);
            _renderTargetQuatered = new RenderTarget2D(GraphicsDevice, width / 4, height / 4, false, format, DepthFormat.None);
            _renderTargetEighted = new RenderTarget2D(GraphicsDevice, width / 8, height / 8, false, format, DepthFormat.None);
            _origTarget = new RenderTarget2D(GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
  
        }

        public void HandleInput(GameTime gameTime)
        {
            if (CurrentGameStatus == GameStatus.Setup)
            {

            }

            else if (CurrentGameStatus == GameStatus.InProgress)
            {

                if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {

                    if ((CurrentVsType == VsType.PlayerVsCpu && _currentTurn == PlayerColor) || CurrentVsType == VsType.PlayerVsPlayer)
                    {
                        Vector2 worldPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(_cam.get_transformation(GraphicsDevice)));
                        CheckerTile tile = _checkerBoard.Board.GetCheckerTile(worldPosition);

                        //Handle mouse input for the game if it is player vs player or (player vs cpu if it is the player's turn)
                        if (CurrentVsType == VsType.PlayerVsPlayer || CurrentVsType == VsType.PlayerVsCpu && _currentTurn == PlayerColor)
                        {
                            //Check that the user clicked on a tile
                            if (tile != null)
                            {
                                //User clicked on his own piece               
                                var checkerPiece = _checkerBoard.GetCheckerPiece(tile);
                                if ((checkerPiece) != null && checkerPiece.Color == _currentTurn)
                                {
                                    _checkerBoard.SelectedPiece = checkerPiece;
                                    if (_selectedParticle != null)
                                    {
                                        _selectedParticle.status = ParticleStatus.Dead;
                                        _selectedParticle = null;
                                    }
                                    _selectedParticle = _particleManager.Spawn(_particleManager.particleSystemInfoDict["fireSmall"], _checkerBoard.GetCenterOfTile(checkerPiece.Row, checkerPiece.Col));
                                }

                                //If a user has selected a piece and the piece is a movable piece
                                if (_checkerBoard.SelectedPiece != null && _checkerBoard.MovablePieces[_currentTurn].Contains(_checkerBoard.SelectedPiece)
                                    && _checkerBoard.MoveDict.ContainsKey(_checkerBoard.SelectedPiece) && _checkerBoard.MoveDict[_checkerBoard.SelectedPiece].Contains(tile))
                                {
                                    //Handle the move to that location
                                    _checkerBoard.HandleMove(tile.Row, tile.Col);
                                    if (_selectedParticle != null)
                                    {
                                        _selectedParticle.status = ParticleStatus.Dead;
                                        _selectedParticle = null;
                                    }
                                    //Move to the next turn
                                    _currentTurn = _checkerBoard.NextTurn(_currentTurn);
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
            TimeElapsed += delta;
            // Allows the game to exit
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    Exit();

                _screenManager.Update(gameTime);
                if (CurrentGameStatus == GameStatus.Setup)
                {
                    if (_setupDialogOff)
                    {
                        _setupDialogOff = false;
                        _setupForm.ShowDialog();
                    }
                    /*
                    Renew();
                    currentGameStatus = GameStatus.IN_PROGRESS;
                     */
                }
                else if (CurrentGameStatus == GameStatus.InProgress)
                {

                    CheckerStatus status = _checkerBoard.GetStatus(_currentTurn);
                    if (status != CheckerStatus.Continue)
                        CurrentGameStatus = GameStatus.GameOver;
                    if (status == CheckerStatus.Win)
                    {
                        _message = _color[_currentTurn] + " Wins!";
                        Console.WriteLine("{0} wins!", _color[_currentTurn]);
                    }
                    else if (status == CheckerStatus.Lose)
                    {
                        _message = String.Format("{0} wins!", _color[(_currentTurn + 1) % 2]);
                        Console.WriteLine(_message);
                    }
                    else
                    {
                        if (CurrentVsType == VsType.CpuVsCpu)
                        {
                            if (_oThread == null)
                            {
                                _oThread = new Thread(Ai);
                                _oThread.Start();
                            }
                        }
                        else if (CurrentVsType == VsType.PlayerVsCpu)
                        {
                            if (PlayerColor != _currentTurn)
                            {
                                if (_oThread == null)
                                {
                                    _oThread = new Thread(Ai);
                                    _oThread.Start();

                                }                            
                            }
                        }

                    }

                    if (_oThread != null)
                    {
                        if (!_oThread.IsAlive)
                        {
                            _oThread.Join();
                            _oThread = null;
                            //Move to the next turn
                            _currentTurn = _checkerBoard.NextTurn(_currentTurn);
                        }
                    }

                    _checkerBoard.Update(gameTime, _cam);
                }
                else if (CurrentGameStatus == GameStatus.GameOver)
                {
                    _timer += delta;
                    if (_particleManager.particleSystems.Count() < MaxFireworks)
                    {
                        if (_timer > .5)
                        {
                            _timer = 0f;
                            _particleManager.Spawn(_particleManager.particleSystemInfoDict["firework"], new Vector2(Rand.Next((int)(GraphicsDevice.Viewport.Width * .1f), (int)(GraphicsDevice.Viewport.Width * .9f)), Rand.Next((int)(GraphicsDevice.Viewport.Height * .1f), (int)(GraphicsDevice.Viewport.Height * .90f))), 1f);
                        }

                    }
                }
                _particleManager.Update(gameTime);
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
            GraphicsDevice.SetRenderTarget(_sceneRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //checkerBoard.Draw(spriteBatch, cam);
            _drawDriver.DrawCheckerBoard(_checkerBoard.Board.TileBoard, _checkerBoard.AllPieces, _checkerBoard.SelectedPiece, _checkerBoard.MovablePieces, _checkerBoard.MoveDict, _checkerBoard.Board.TileScale, _spriteBatch, _cam);
            _particleManager.Draw(_spriteBatch, _cam);
            if (CurrentGameStatus == GameStatus.Setup)
            {
                
            }
            else if (CurrentGameStatus == GameStatus.InProgress)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("{0}'s turn", _color[_currentTurn]);
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, (stringBuilder), new Vector2(GraphicsDevice.Viewport.Width - 149, 10), Color.Black);
                _spriteBatch.End();
            }
            else if (CurrentGameStatus == GameStatus.GameOver)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, _message, new Vector2(GraphicsDevice.Viewport.Width - 149, 10), Color.Black);
                _spriteBatch.End();
            }
            
            _screenManager.Draw(_spriteBatch);
            bolt.Draw();
            DrawFullscreenQuad(_sceneRenderTarget, _renderTargetHalved, Colorize);
            DrawFullscreenQuad(_renderTargetHalved, _renderTargetQuatered, null);
            DrawFullscreenQuad(_renderTargetQuatered, _renderTargetEighted, null);

            DrawSceneWithBlur(_renderTargetEighted, _origTarget);
            DrawSceneWithBlur(_renderTargetQuatered, _origTarget);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_renderTargetEighted, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_renderTargetQuatered, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            //spriteBatch.Draw(renderTargetHalved, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_sceneRenderTarget, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        //Alpha Beta

        void Ai()
        {
            _bestMove = null;
            _bestPiece = null;
            _nodeGeneration = 0;
            _maxPruning = 0;
            _minPruning = 0;
            _maxDepth = 0;
            AlphaBetaSearch(_checkerBoard, _currentTurn);
            //If it never found something b/c it was limited in depth
            if (_bestMove == null && _bestPiece == null){
            
                var randomPiece = Rand.Next(0, _checkerBoard.MovablePieces[_currentTurn].Count - 1);
                var randomMove = Rand.Next(0, _checkerBoard.MoveDict[_checkerBoard.MovablePieces[_currentTurn][randomPiece]].Count - 1);
                _bestMove = _checkerBoard.MoveDict[_checkerBoard.MovablePieces[_currentTurn][randomPiece]][randomMove];
                _bestPiece = _checkerBoard.MovablePieces[_currentTurn][randomPiece];
            }
            _checkerBoard.SelectedPiece = _bestPiece;
            if (_bestPiece != null)
                Console.WriteLine("Moving Piece at Row: {0}, Col: {1}", _bestPiece.Row, _bestPiece.Col);
            if (_bestMove != null) 
                Console.WriteLine("To Row: {0}, Col: {1}", _bestMove.Row, _bestMove.Col);
            Console.WriteLine("Max Depth: {0}", _maxDepth);
            Console.WriteLine("# of Pruning in Max: {0}", _maxPruning);
            Console.WriteLine("# of Pruning in Min: {0}", _minPruning);
            Console.WriteLine("# of Nodes Generated: {0}", _nodeGeneration);
            Console.WriteLine("Time Elapsed: {0}", TimeElapsed);
            
            if (_selectedParticle != null)
            {
                _selectedParticle.status = ParticleStatus.Dead;
                _selectedParticle = null;
            }
            if (_bestPiece != null)
                _selectedParticle = _particleManager.Spawn(_particleManager.particleSystemInfoDict["fireSmall"], _checkerBoard.GetCenterOfTile(_bestPiece.Row, _bestPiece.Col));

            Thread.Sleep(TimeDelay);
            //Handle the move to that location
            Debug.Assert(_bestMove != null, "bestMove != null");
            _checkerBoard.HandleMove(_bestMove.Row, _bestMove.Col);



            if (_selectedParticle != null)
            {
                _selectedParticle.status = ParticleStatus.Dead;
                _selectedParticle = null;
            }

        }
        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, Effect effect)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            _spriteBatch.Draw(texture, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
            _spriteBatch.End();
        }
        public void DrawSceneWithBlur(RenderTarget2D scene, RenderTarget2D targetB)
        {
            SetBlurEffectParameters(1.0f / (float)scene.Width, 0);
            DrawFullscreenQuad(scene, targetB, GaussianBlur);

            SetBlurEffectParameters(0, 1.0f / (float)scene.Height);
            DrawFullscreenQuad(targetB, scene, GaussianBlur);
        }
        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            float theta = 3f;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
        void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = GaussianBlur.Parameters["SampleWeights"];
            offsetsParameter = GaussianBlur.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(1);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }
        void AlphaBetaSearch(CheckerBoard currentBoard, int playerColor)
        {
            _maxPruning = 0;
            _minPruning = 0;
            _maxDepth = 0;
            TimeElapsed = 0;
            MaxValue(currentBoard, 0, -999, 999, playerColor);           
        }
        //Gets the Max Value
        int MaxValue(CheckerBoard currentBoard, int depth, int alpha, int beta, int currentPlayer)
        {
            if (_nodeGeneration % 10000 == 0 && _nodeGeneration !=0)
            {
                Console.WriteLine("Max Depth: {0}", _maxDepth);
                Console.WriteLine("# of Pruning in Max: {0}", _maxPruning);
                Console.WriteLine("# of Pruning in Min: {0}", _minPruning);
                Console.WriteLine("# of Node Generated: {0}", _nodeGeneration);
                Console.WriteLine("Time Elapsed: {0}", TimeElapsed);
            }
            //Checks to see if it is a utility value
            CheckerStatus status = currentBoard.GetStatus(_currentTurn);

            //If it is return the value
            if (status != CheckerStatus.Continue)
            {
                return (int) status;
            }

            _maxDepth = Math.Max(_maxDepth, depth);

            //Depth Limiter
            if (depth == Difficulty)
                return currentBoard.EvaluateBoard(_currentTurn);
            var v = -999;

            //Iterate through every movable pieces
            for (var i = 0; i < currentBoard.MovablePieces[currentPlayer].Count(); i++)
            {
                //Iterate through every possible move for the selected piece
                for (var j = 0; j < currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]].Count(); j++)
                {
                    //Increment node counter
                    _nodeGeneration++;
                    //For each possible move make a new checkerboard and move it
                    var newCheckerBoard = new CheckerBoard(currentBoard);

                    //Select the piece that will be moved
                    var selectedPiece = newCheckerBoard.GetCheckerPiece(currentBoard.MovablePieces[currentPlayer][i].Row, currentBoard.MovablePieces[currentPlayer][i].Col);
                    newCheckerBoard.SelectedPiece = selectedPiece;

                    //Move the piece to a piece location
                    newCheckerBoard.HandleMove(currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Row, currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Col);


                    newCheckerBoard.CheckAllAvailableMoves();
                    
                    var nextTurn = newCheckerBoard.NextTurn(currentPlayer);

                    if (nextTurn == currentPlayer)
                    {
                        v = Math.Max(v, MaxValue(newCheckerBoard, depth, alpha, beta, nextTurn));
                    }
                    else
                        v = Math.Max(v, MinValue(newCheckerBoard, depth + 1, alpha, beta, nextTurn));
                    
                    

                    if (v >= beta)
                    {
                        _maxPruning++;
                        return v;
                    }

                    if (v > alpha)
                    {
                        alpha = v;
                        if (currentBoard == _checkerBoard)
                        {
                            _bestMove = currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j];
                            _bestPiece = currentBoard.MovablePieces[currentPlayer][i];
                        }
                    }
                }
            }
            return v;
        }
        int MinValue(CheckerBoard currentBoard, int depth, int alpha, int beta, int currentPlayer)
        {
            if (_nodeGeneration % 10000 == 0 && _nodeGeneration != 0)
            {
                Console.WriteLine("Max Depth: {0}", _maxDepth);
                Console.WriteLine("# of Pruning in Max: {0}", _maxPruning);
                Console.WriteLine("# of Pruning in Min: {0}", _minPruning);
                Console.WriteLine("# of Node Generated: {0}", _nodeGeneration);
                Console.WriteLine("Time Elapsed: {0}", TimeElapsed);
            }
            CheckerStatus status = currentBoard.GetStatus(_currentTurn);

            if (status != CheckerStatus.Continue)
            {
                return (int) status;
            }

            _maxDepth = Math.Max(_maxDepth, depth);
            if (depth == Difficulty)
                return currentBoard.EvaluateBoard(_currentTurn); 
            int v = 999;
            //For each movable piece
            for (var i = 0; i < currentBoard.MovablePieces[currentPlayer].Count(); i++)
            {
                //for each possible positions that the piece can go
                for (var j = 0; j < currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]].Count(); j++)
                {
                    _nodeGeneration++;
                    //For each possible move make a new checkerboard and move it
                    var newCheckerBoard = new CheckerBoard(currentBoard);
                    var selectedPiece = newCheckerBoard.GetCheckerPiece(currentBoard.MovablePieces[currentPlayer][i].Row, currentBoard.MovablePieces[currentPlayer][i].Col);
                    newCheckerBoard.SelectedPiece = selectedPiece;
                    newCheckerBoard.HandleMove(currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Row, currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j].Col);
                    newCheckerBoard.CheckAllAvailableMoves();
                    int nextTurn = newCheckerBoard.NextTurn(currentPlayer);

                    if(nextTurn == currentPlayer)
                    {
                        v = Math.Min(v, MinValue(newCheckerBoard, depth, alpha, beta, nextTurn));
                    }
                    else
                        v = Math.Min(v, MaxValue(newCheckerBoard, depth + 1, alpha, beta, nextTurn));
                    //pruning
                    if (v <= alpha)
                    {
                        
                        _minPruning++;                        
                        return v;
                    }

                    if (v < beta)
                    {
                        beta = v;
                        if (currentBoard == _checkerBoard)
                        {
                            _bestMove = currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]][j];
                            _bestPiece = currentBoard.MovablePieces[currentPlayer][i];
                        }
                    }
                }
            }
            return v;
        }

        public void Renew()
        {
            _message = "";       
            _timer = 0;
            _nodeGeneration = 0;
            _currentTurn = 1;
            _checkerBoard.Reset(Rows, Cols);
            _cam.Zoom = (float)WINDOW_HEIGHT/(TileScale * Rows);
            if (_selectedParticle != null)
            {
                _selectedParticle.status = ParticleStatus.Dead;
            }
        }
        

    }
}
