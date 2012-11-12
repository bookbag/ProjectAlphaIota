using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ProjectAlphaIota
{
    class CheckerBoard
    {
        
        int TILE_SCALE;
        CheckerTile[,] tileBoard;
        int rows, cols;
        Texture2D blankTexture;
        bool contentLoaded;
        float halfWidth, halfHeight;
        float centerX, centerY;
        GraphicsDevice device;

        SpriteFont font;

        //Checker Piece variables

        bool[] mustJump = new bool[2]{false, false};
        List<CheckerPiece>[] movablePieces = null;
        Dictionary<CheckerPiece, List<CheckerTile>> moveDict = null;
        CheckerPiece selectedPiece = null;
        List<CheckerPiece> allPieces = null;
        CheckerPiece jumpMade = null;

        public List<CheckerPiece>[] MovablePieces
        {
            get { return movablePieces; }
            set { movablePieces = value; }
        }
        public Dictionary<CheckerPiece, List<CheckerTile>> MoveDict
        {
            get { return moveDict; }
            set { moveDict = value; }
        }
        public CheckerPiece SelectedPiece
        {
            get { return selectedPiece; }
            set { selectedPiece = value; }
        }

        //copy constructor
        public CheckerBoard(CheckerBoard other)
        {
            this.rows = other.rows;
            this.cols = other.cols;
            this.TILE_SCALE = other.TILE_SCALE;
            this.halfWidth = rows * TILE_SCALE / 2;
            this.halfHeight = cols * TILE_SCALE / 2;

            this.centerX = other.centerX;
            this.centerY = other.centerY;
            tileBoard = new CheckerTile[rows, cols];
            contentLoaded = false;

            tileBoard = other.tileBoard;
            mustJump = new bool[2] { other.mustJump[0], other.mustJump[1] };
            selectedPiece = null;
            jumpMade = other.jumpMade;
        }
        public CheckerBoard(int rows, int cols, float centerX, float centerY, int TILE_SCALE = 50)
        {
            this.rows = rows;
            this.cols = cols;
            
            this.TILE_SCALE = TILE_SCALE;

            halfWidth = rows * TILE_SCALE / 2;
            halfHeight = cols * TILE_SCALE / 2;

            this.centerX = centerX;
            this.centerY = centerY;

            tileBoard = new CheckerTile[rows, cols];
            contentLoaded = false;

            bool black = true;
            allPieces = new List<CheckerPiece>();
            for (int row = 0; row < rows; row++)    //top to down
            {
                for (int col = cols - 1; col >= 0; col--) // right to left
                {
                    if (black)
                    {
                        tileBoard[row, col] = new CheckerTile(row, col, 1);
                    }
                    else
                        tileBoard[row, col] = new CheckerTile(row, col, 0);


                    black = !black;
                    if (col == 0)
                        black = !black;
                }
            }
            Reset();
        }        

        //checks the status of a checkerboard state, 0 = draw, -1 = lose, 1 = win, 2 = continue
        public CheckerStatus GetStatus(int playerColor)
        {
            //Game is a Draw
            if (movablePieces[0].Count == 0 && movablePieces[1].Count == 0)
            {
                return CheckerStatus.DRAW;
            }

            //Count the number of pieces of a color
            int numWhite = 0;
            for (int i = 0; i < allPieces.Count; i++)
            {
                if (allPieces[i].Color == 0)
                    numWhite++;
            }
            
            //Win/Lose
            if (numWhite == allPieces.Count )
            {
                if (playerColor == 0)
                {
                    return CheckerStatus.WIN;
                }
                else
                {
                    return CheckerStatus.LOSE;
                }
            }
            else if (numWhite == 0 )
            {
                if (playerColor == 1)
                {
                    return CheckerStatus.WIN;
                }
                else
                {
                    return CheckerStatus.LOSE;
                }
            }
            return CheckerStatus.CONTINUE; 
            
        }
        public void Reset()
        {
            //Place the pieces
            bool black = true;
            allPieces = new List<CheckerPiece>();
            for (int row = 0; row < rows; row++)    //top to down
            {
                for (int col = cols - 1; col >= 0; col--) // right to left
                {
                    if (black)
                    {
                        int num_piece_rows = (int)((rows - 2) / 2);
                        if (row < num_piece_rows)
                        {
                            CheckerPiece tempPiece = new CheckerPiece(row, col, 0);
                            allPieces.Add(tempPiece);
                        }
                        else if (row >= rows - num_piece_rows)
                        {
                            CheckerPiece tempPiece = new CheckerPiece(row, col, 1);
                            allPieces.Add(tempPiece);
                        }
                    }

                    black = !black;
                    if (col == 0)
                        black = !black;
                }
            }
            CheckAllAvailableMoves();
        }
        public void LoadContent(GraphicsDevice device, ContentManager contentManager, ParticleManager particleManager)
        {
            this.device = device;
            blankTexture = new Texture2D(device, 1, 1);
            blankTexture.SetData<Color>(new Color[] { Color.White });
            contentLoaded = true;
            font = contentManager.Load<SpriteFont>("SpriteFont1");
        }
        public CheckerPiece GetPiece(int row, int col)
        {
            for (int i = 0; i < allPieces.Count(); i++)
            {
                if (allPieces[i].Row == row && allPieces[i].Col == col)
                {
                    return allPieces[i];
                }
            }
            return null;
        }
        public CheckerTile GetCheckerTile(Vector2 position)
        {
            int row = (int)((position.Y + halfHeight) / TILE_SCALE);
            int col = (int)((position.X + halfWidth) / TILE_SCALE);
            if(checkValidTile(row, col))
            {
                return tileBoard[row, col];
            }
            return null;
        }
        public CheckerPiece GetCheckerPiece(Vector2 position)
        {
            int row = (int)((position.Y + halfHeight) / TILE_SCALE);
            int col = (int)((position.X + halfWidth) / TILE_SCALE);
            if(checkValidTile(row, col))
            {
                return GetPiece(row, col);
            }
            return null;
        }
        public CheckerPiece GetCheckerPiece(CheckerTile tile)
        {
            return GetPiece(tile.Row, tile.Col);
        }
        public bool checkValidTile(int row, int col)
        {
            return row >= 0 && col >= 0 && row < rows && col < cols;
        }
        public Vector2 getCenterOfTile(int row, int col)
        {
            return new Vector2((float)((-halfWidth) + col * TILE_SCALE + .5f * TILE_SCALE), (float)((-halfHeight) + row * TILE_SCALE + .5f * TILE_SCALE));
        }
        public void CheckAllAvailableMoves()
        {
            
            movablePieces = new List<CheckerPiece>[2];
            movablePieces[0] = new List<CheckerPiece>();
            movablePieces[1] = new List<CheckerPiece>();
            moveDict = new Dictionary<CheckerPiece, List<CheckerTile>>();
            selectedPiece = null;
            
            for (int i = 0; i < allPieces.Count; i++)
            {
                CheckAvailableMoves(allPieces[i]);
            }
            
        }
        public void CheckAvailableMoves(CheckerPiece piece)
        {
            int row = piece.Row;
            int col = piece.Col;
            List<CheckerTile> tempPossibleMoves = new List<CheckerTile>();
            CheckerPiece targetPiece;
            //Check if there is a jump move
            //At most there are four jump moves with a piece in between and a empty piece after
            //Check if the tile is valid and has a piece on it and the color is not yours

            if (checkValidTile(row + 1, col - 1))
            {
                targetPiece = GetCheckerPiece(tileBoard[row + 1, col - 1]);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (checkValidTile(row + 2, col - 2) && GetCheckerPiece(tileBoard[row + 2, col - 2]) == null)
                    {
                        tempPossibleMoves.Add(tileBoard[row + 2, col - 2]);
                    }
                }
            }
            if (checkValidTile(row + 1, col + 1))
            {
                targetPiece = GetCheckerPiece(tileBoard[row + 1, col + 1]);
                if(targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (checkValidTile(row + 2, col + 2) && GetCheckerPiece(tileBoard[row + 2, col + 2]) == null)
                    {
                        tempPossibleMoves.Add(tileBoard[row + 2, col + 2]);
                    }
                }
            }
            
            if (checkValidTile(row - 1, col - 1))
            {
                targetPiece = GetCheckerPiece(tileBoard[row - 1, col - 1]);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (checkValidTile(row - 2, col - 2) && GetCheckerPiece(tileBoard[row - 2, col - 2]) == null)
                    {
                        tempPossibleMoves.Add(tileBoard[row - 2, col - 2]);
                    }
                }
            }
            
            if (checkValidTile(row - 1, col + 1))
            {
                targetPiece = GetCheckerPiece(tileBoard[row - 1, col + 1]);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (checkValidTile(row - 2, col + 2) && GetCheckerPiece(tileBoard[row - 2, col + 2]) == null)
                    {
                        tempPossibleMoves.Add(tileBoard[row - 2, col + 2]);
                    }
                }
            }
        
            //regular move
            //Check for regular move if there is no jump moves
            if (tempPossibleMoves.Count() == 0)
            {
                if (mustJump[piece.Color] == false)
                {
                    //If the current turn is white
                    if (piece.Color == 0)
                    {
                        //At most there are two regular moves left or right diagonal

                        //Check if the tile is valid and has no piece on it
                        if (checkValidTile(row + 1, col - 1) && GetCheckerPiece(tileBoard[row + 1, col - 1]) == null)
                        {
                            tempPossibleMoves.Add(tileBoard[row + 1, col - 1]);
                        }
                        if (checkValidTile(row + 1, col + 1) && GetCheckerPiece(tileBoard[row + 1, col + 1]) == null)
                        {
                            tempPossibleMoves.Add(tileBoard[row + 1, col + 1]);
                        }
                    }
                    //current turn is black
                    else
                    {
                        //At most there are two regular moves left or right diagonal

                        //Check if the tile is valid and has no piece on it
                        if (checkValidTile(row - 1, col - 1) && GetCheckerPiece(tileBoard[row - 1, col - 1]) == null)
                        {
                            tempPossibleMoves.Add(tileBoard[row - 1, col - 1]);
                        }
                        if (checkValidTile(row - 1, col + 1) && GetCheckerPiece(tileBoard[row - 1, col + 1]) == null)
                        {
                            tempPossibleMoves.Add(tileBoard[row - 1, col + 1]);
                        }
                    }
                }
            }
            //A jump can be made
            else
            {
                //A previous jump was made
                if (jumpMade != null)
                {
                    if (jumpMade == piece)
                    {
                        MovablePieces[piece.Color].Add(piece);
                        mustJump[piece.Color] = true;
                    }
                }
                //If there was no previous jump made
                else
                {
                    //If this is the first jump
                    if (mustJump[piece.Color] == false)
                    {
                        //Clear the movable piece set
                        MovablePieces[piece.Color].Clear();
                        mustJump[piece.Color] = true;
                    }
                    //if this piece is a jump piece add it
                    if (mustJump[piece.Color])
                    {
                        MovablePieces[piece.Color].Add(piece);
                    }
                }
                
            }
            //Add this to the dictionary if there is actual moves
            if (tempPossibleMoves.Count != 0)
            {
                MoveDict[piece] = tempPossibleMoves;
                //if there is no jump add it
                if (!mustJump[piece.Color])
                    MovablePieces[piece.Color].Add(piece);
            }
        }
        public void HandleMove(int newRow, int newCol)
        {
            //Remove the jumped piece
            if (mustJump[selectedPiece.Color])
            {
                for (int i = 0; i < allPieces.Count(); i++)
                {
                    if (allPieces[i].Row == (newRow + selectedPiece.Row) / 2 && allPieces[i].Col == (newCol + selectedPiece.Col) / 2)
                    {
                        allPieces.RemoveAt(i);
                    }
                }
                jumpMade = selectedPiece;
            }
            //Change the piece's position;
            selectedPiece.Row = newRow;
            selectedPiece.Col = newCol;

           
        }
        public void NextTurn(ref int currentTurn)
        {
            if (jumpMade != null)
            {
                CheckAllAvailableMoves();
                Console.WriteLine("A jump was made. Re-evaluating jump possibility for jumped piece.\n");
            }
            if (jumpMade == null || !MovablePieces[jumpMade.Color].Contains(jumpMade))
            {
                mustJump[0] = false;
                mustJump[1] = false;
                jumpMade = null;
                currentTurn = (currentTurn + 1) % 2;
                Console.WriteLine("Player {0}'s Turn.\n", currentTurn);
                CheckAllAvailableMoves();
            }

        }
        /*
        public void HandleMove(ref CheckerPiece selectedPiece, ref Dictionary<CheckerPiece, List<CheckerTile>> possibleMoves, int row, int col, ref CheckerTile[,] tileBoard, ref List<CheckerPiece> allPieces)
        {
            //Remove the jumped piece
            if (canJump.Count > 0 && canJump.Contains(selectedPiece))
            {
                allPieces.Remove(tileBoard[(row + selectedPiece.Row) / 2, (col + selectedPiece.Col) / 2].Piece);
                tileBoard[(row + selectedPiece.Row) / 2, (col + selectedPiece.Col) / 2].Piece = null;
                jumpMade = selectedPiece;
            }
            //Change the piece's position;
            selectedPiece.Row = row;
            selectedPiece.Col = col;

            //Set the new tile with the moved piece
            tileBoard[row, col].Piece = selectedPiece;

            //Remove the piece from the tile
            selectedPiece = null;

            //Reset the selection, moves, and particle
            selectedPiece = null;
            possibleMoves = null;
            selectedParticle.status = STATUS.DEAD;
            selectedParticle = null;
        }
        public void HandleCPUMove()
        {
            //Check to see if there's a jump
            if (canJump.Count() != 0)
            {
                //Find 
            }
        }
         */

        public bool canMove(int playerColor)
        {
            return (movablePieces[playerColor].Count() != 0);
        }
        public void Update(GameTime gameTime, Camera cam)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!contentLoaded)
                return;
        }
        public void DrawPossibleMoves(SpriteBatch spriteBatch, Camera cam, List<CheckerTile> allTile)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(device));
            for (int i = 0; i < allTile.Count(); i++)
            {
                spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + allTile[i].Col * TILE_SCALE) + 10, (int)(-halfHeight) + allTile[i].Row * TILE_SCALE + 10, TILE_SCALE - 20, TILE_SCALE - 20), null, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 0);                
            }
            spriteBatch.End();
        }
        public void DrawPieces(SpriteBatch spriteBatch, Camera cam, List<CheckerPiece> allPieces)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(device));
            for (int i = 0; i < allPieces.Count(); i++)
            {
                CheckerPiece currentPiece = allPieces[i];
                if (currentPiece != null)
                {
                    if (currentPiece.Color == 0)
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + currentPiece.Col * TILE_SCALE) + 10, (int)(-halfHeight) + currentPiece.Row * TILE_SCALE + 10, TILE_SCALE - 20, TILE_SCALE - 20), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + currentPiece.Col * TILE_SCALE) + 10, (int)(-halfHeight) + currentPiece.Row * TILE_SCALE + 10, TILE_SCALE - 20, TILE_SCALE - 20), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
                }
                    
            }
            spriteBatch.End();
        }
        public void Draw(SpriteBatch spriteBatch, Camera cam)
        {
            if (!contentLoaded)
                return;
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          null,
                          null,
                          null,
                          null,
                          cam.get_transformation(device));
            //Checker Tiles           
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (tileBoard[row, col].Color == 1)
                    {
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + col * TILE_SCALE), (int)(-halfHeight) + row * TILE_SCALE, TILE_SCALE, TILE_SCALE), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);                        
                    }
                    else
                    {
                        spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + col * TILE_SCALE), (int)(-halfHeight) + row * TILE_SCALE, TILE_SCALE, TILE_SCALE), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
                    }
           
                    
                }
            }
            //Lines
            for (int row = 0; row <= rows; row++)
            {
                spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth), (int)(-halfHeight) + row * TILE_SCALE, cols * TILE_SCALE, 1), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            for (int col = 0; col <= cols; col++)
            {
                spriteBatch.Draw(blankTexture, new Rectangle((int)(-halfWidth + col * TILE_SCALE), (int)(-halfHeight), 1, cols * TILE_SCALE), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            DrawPieces(spriteBatch, cam, allPieces);

            //Draw the possible moves
            if (selectedPiece != null && movablePieces != null && movablePieces[selectedPiece.Color].Contains(selectedPiece) && MoveDict.ContainsKey(selectedPiece))
            {
                DrawPossibleMoves(spriteBatch, cam, MoveDict[selectedPiece]);
            }

           
        }
    }
}
