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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        CheckerBoard checkerBoard;
        ContentManager contentManager;
        ParticleManager particleManager;
        Camera cam;
        SpriteFont font;

        ParticleSystem selectedParticle = null;

        

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
        int playerColor = 1; //The player's color
        VS_TYPE vs_type = VS_TYPE.PLAYER_VS_PLAYER; //Player vs CPU as opposed to CPU vs CPU
        string[] color = new string[2]{"white", "black"};
        string message = "";

        GameStatus currentGameStatus = GameStatus.SETUP;

        public CheckerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            contentManager = new ContentManager(Services);
            contentManager.RootDirectory = "Content";
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
            checkerBoard = new CheckerBoard(6, 6, 0, 0);
            cam = new Camera();
            cam.Pos = new Vector2(0,0);
            particleManager = new ParticleManager();
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
            particleManager.LoadContent(contentManager, GraphicsDevice);
            checkerBoard.LoadContent(GraphicsDevice, contentManager, particleManager);
            font = contentManager.Load<SpriteFont>("SpriteFont1");
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
            if(currentGameStatus== GameStatus.SETUP)
            {
                
            }

            else if (currentGameStatus == GameStatus.IN_PROGRESS)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    
                    if ((vs_type == VS_TYPE.PLAYER_VS_CPU && currentTurn == playerColor) || vs_type == VS_TYPE.PLAYER_VS_PLAYER)
                    {
                        Vector2 worldPosition = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(cam.get_transformation(GraphicsDevice)));
                        CheckerTile tile = checkerBoard.GetCheckerTile(worldPosition);

                        //Handle mouse input for the game if it is player vs player or (player vs cpu if it is the player's turn)
                        if (vs_type == VS_TYPE.PLAYER_VS_PLAYER || vs_type == VS_TYPE.PLAYER_VS_CPU && currentTurn == playerColor)
                        {    
                            //Check that the user clicked on a tile
                            if (tile != null)
                            {
                                //User clicked on his own piece               
                                CheckerPiece checkerPiece = null;
                                if ((checkerPiece = checkerBoard.GetCheckerPiece(tile)) != null && checkerPiece.Color == currentTurn)
                                    checkerBoard.SelectedPiece = checkerPiece;

                                //If a user has selected a piece and the piece is a movable piece
                                if (checkerBoard.SelectedPiece != null && checkerBoard.MovablePieces[currentTurn].Contains(checkerBoard.SelectedPiece) 
                                    && checkerBoard.MoveDict.ContainsKey(checkerBoard.SelectedPiece) && checkerBoard.MoveDict[checkerBoard.SelectedPiece].Contains(tile))
                                {
                                    //Handle the move to that location
                                    checkerBoard.HandleMove(tile.Row, tile.Col);
                                    if (selectedParticle != null)
                                    {
                                        selectedParticle.status = STATUS.DEAD;
                                        selectedParticle = null;
                                    }
                                    //Move to the next turn
                                    checkerBoard.NextTurn(ref currentTurn);
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
            // Allows the game to exit
            if (IsActive)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                

                if(currentGameStatus== GameStatus.SETUP)
                {
                    renew();
                    currentGameStatus = GameStatus.IN_PROGRESS;
                }
                else if (currentGameStatus == GameStatus.IN_PROGRESS)
                {
                    
                    CheckerStatus status = checkerBoard.GetStatus(currentTurn);
                    if(status!= CheckerStatus.CONTINUE)
                        currentGameStatus = GameStatus.GAME_OVER;
                    if (status == CheckerStatus.WIN)
                    {
                        message = color[currentTurn] + " Wins!";
                        Console.WriteLine("{0} Wins!", color[currentTurn]);
                    }
                    else if (status == CheckerStatus.LOSE)
                    {
                        message = color[(currentTurn + 1) % 2] + " Wins!";
                        Console.WriteLine("{0} Wins!", color[(currentTurn + 1) % 2]);
                    }
                    else if (status == CheckerStatus.DRAW)
                    {
                        message = "DRAW";
                        Console.WriteLine("Draw");
                    }
                    else
                    {
                        if (!checkerBoard.canMove(currentTurn))
                        {
                            checkerBoard.NextTurn(ref currentTurn);
                        }
                    }
                    
                    checkerBoard.Update(gameTime, cam);
                }
                else if (currentGameStatus == GameStatus.GAME_OVER)
                {

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

            checkerBoard.Draw(spriteBatch, cam);
            particleManager.Draw(spriteBatch, cam);
            if (currentGameStatus == GameStatus.SETUP)
            {
            }
            else if (currentGameStatus == GameStatus.IN_PROGRESS)
            {
                
            }
            else if (currentGameStatus == GameStatus.GAME_OVER)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(GraphicsDevice));
                spriteBatch.DrawString(font, message, new Vector2(0, 0), Color.Black);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        //Alpha Beta
        int alphabeta(CheckerBoard currentBoard, int depth, int alpha, int beta, int currentPlayer)
        {
            //checkerBoard.CheckAllAvailableMoves(allPieces, out canJump, out MovablePieces, out MoveDict); 
            CheckerStatus status = checkerBoard.GetStatus(currentPlayer);
            if (status != CheckerStatus.CONTINUE)
                return (int)status;
            if (currentPlayer == currentTurn) //if player is the current player who wants to maximize
            {
                for (int i = 0; i < currentBoard.MovablePieces[currentPlayer].Count(); i++)
                {
                    for (int j = 0; j < currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]].Count(); j++)
                    {
                        //For each possible move make a new checkerboard and move it
                        CheckerBoard newCheckerBoard = new CheckerBoard(currentBoard);
                        CheckerPiece selectedPiece = currentBoard.MovablePieces[currentPlayer][i];
                        newCheckerBoard.SelectedPiece = selectedPiece;
                        newCheckerBoard.HandleMove(currentBoard.MoveDict[selectedPiece][j].Row, currentBoard.MoveDict[selectedPiece][j].Col);
                        newCheckerBoard.CheckAllAvailableMoves();
                        //Have to check for all jump paths =[
                        //newCheckerBoard.NextTurn

                        alpha = Math.Max(alpha, alphabeta(newCheckerBoard, depth - 1, alpha, beta, (currentPlayer + 1) % 2));
                        if (beta <= alpha)
                            break;
                    }
                    if (beta <= alpha)
                        break;
                }
                return alpha;
            }
            else
            {
                for (int i = 0; i < currentBoard.MovablePieces[currentPlayer].Count(); i++)
                {
                    for (int j = 0; j < currentBoard.MoveDict[currentBoard.MovablePieces[currentPlayer][i]].Count(); j++)
                    {
                        //For each possible move make a new checkerboard and move it
                        CheckerBoard newCheckerBoard = new CheckerBoard(currentBoard);
                        CheckerPiece selectedPiece = currentBoard.MovablePieces[currentPlayer][i];
                        newCheckerBoard.SelectedPiece = selectedPiece;
                        checkerBoard.HandleMove(currentBoard.MoveDict[selectedPiece][j].Row, currentBoard.MoveDict[selectedPiece][j].Col);

                        beta = Math.Max(beta, alphabeta(newCheckerBoard, depth - 1, alpha, beta, (currentPlayer + 1) % 2));
                        if (beta <= alpha)
                            break;
                    }
                    if (beta <= alpha)
                        break;
                }
                return beta;
            }

        }

        public void renew()
        {
            message = "";
            currentTurn = 1;
            checkerBoard.Reset();

            if (selectedParticle != null)
            {
                selectedParticle.status = STATUS.DEAD;
            }            
        }

        
    }
}
