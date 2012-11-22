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

        public int TILE_SCALE { get; set; }
        public CheckerTile[,] TileBoard { get; set; }
        int rows, cols;
        
        float halfWidth, halfHeight;
        float centerX, centerY;
        
        //Checker Piece variables

        bool[] mustJump = new bool[2]{false, false};
        CheckerPiece jumpMade = null;

        public List<CheckerPiece> AllPieces { get; set; }
        public List<CheckerPiece>[] MovablePieces { get; set; }
        public Dictionary<CheckerPiece, List<CheckerTile>> MoveDict { get; set; }
        public CheckerPiece SelectedPiece { get; set; }
        public Dictionary<Point, CheckerPiece> CheckerPieceMap { get; set; }

        //copy constructor
        public CheckerBoard(CheckerBoard other)
        {
            this.rows = other.rows;
            this.cols = other.cols;
            this.TILE_SCALE = other.TILE_SCALE;
            this.halfWidth = 0;
            this.halfHeight = 0;

            this.centerX = other.centerX;
            this.centerY = other.centerY;
            TileBoard = other.TileBoard;

            TileBoard = other.TileBoard;
            mustJump = new bool[2] { other.mustJump[0], other.mustJump[1] };
            SelectedPiece = null;
            jumpMade = null;
            AllPieces = new List<CheckerPiece>(other.AllPieces.Count());
            for (int i = 0; i < other.AllPieces.Count(); i++)
            {
                AllPieces.Add(new CheckerPiece(other.AllPieces[i].Row, other.AllPieces[i].Col, other.AllPieces[i].Color));
                if (other.jumpMade != null)
                {
                    if (other.AllPieces[i] == other.jumpMade)
                    {
                        jumpMade = AllPieces[i];
                    }
                }
            }            
        }
        public CheckerBoard(int rows, int cols, float centerX, float centerY, int TILE_SCALE = 80)
        {
            this.rows = rows;
            this.cols = cols;
            
            this.TILE_SCALE = TILE_SCALE;

            TileBoard = new CheckerTile[rows, cols];

            bool black = true;
            AllPieces = new List<CheckerPiece>();
            for (int row = 0; row < rows; row++)    //top to down
            {
                for (int col = cols - 1; col >= 0; col--) // right to left
                {
                    if (black)
                    {
                        TileBoard[row, col] = new CheckerTile(row, col, 1);
                    }
                    else
                        TileBoard[row, col] = new CheckerTile(row, col, 0);


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
            if (MovablePieces[playerColor].Count == 0)
            {
                return CheckerStatus.LOSE;
            }

            //Count the number of pieces of a color
            int numWhite = 0;
            for (int i = 0; i < AllPieces.Count; i++)
            {
                if (AllPieces[i].Color == 0)
                    numWhite++;
            }
            
            //Win/Lose
            if (numWhite == AllPieces.Count )
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
            AllPieces = new List<CheckerPiece>();
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
                            AllPieces.Add(tempPiece);
                        }
                        else if (row >= rows - num_piece_rows)
                        {
                            CheckerPiece tempPiece = new CheckerPiece(row, col, 1);
                            AllPieces.Add(tempPiece);
                        }
                    }

                    black = !black;
                    if (col == 0)
                        black = !black;
                }
            }
            CheckAllAvailableMoves();
        }
        
        public CheckerPiece GetCheckerPiece(int row, int col)
        {
            for (int i = 0; i < AllPieces.Count(); i++)
            {
                if (AllPieces[i].Row == row && AllPieces[i].Col == col)
                {
                    return AllPieces[i];
                }
            }
            return null;
        }
        public CheckerTile GetCheckerTile(int row, int col)
        {
            if(CheckValidTile(row, col))
                return TileBoard[row, col];
            return null;
        }
        public CheckerTile GetCheckerTile(Vector2 position)
        {
            int row = (int)((position.Y + halfHeight) / TILE_SCALE);
            int col = (int)((position.X + halfWidth) / TILE_SCALE);
            if(CheckValidTile(row, col))
            {
                return TileBoard[row, col];
            }
            return null;
        }
        public CheckerPiece GetCheckerPiece(Vector2 position)
        {
            int row = (int)((position.Y + halfHeight) / TILE_SCALE);
            int col = (int)((position.X + halfWidth) / TILE_SCALE);
            if(CheckValidTile(row, col))
            {
                return GetCheckerPiece(row, col);
            }
            return null;
        }
        public CheckerPiece GetCheckerPiece(CheckerTile tile)
        {
            return GetCheckerPiece(tile.Row, tile.Col);
        }
        public bool CheckValidTile(int row, int col)
        {
            return row >= 0 && col >= 0 && row < rows && col < cols;
        }
        public Vector2 GetCenterOfTile(int row, int col)
        {
            return new Vector2((float)((-halfWidth) + col * TILE_SCALE + .5f * TILE_SCALE), (float)((-halfHeight) + row * TILE_SCALE + .5f * TILE_SCALE));
        }
        public void CheckAllAvailableMoves()
        {
            
            MovablePieces = new List<CheckerPiece>[2];
            MovablePieces[0] = new List<CheckerPiece>();
            MovablePieces[1] = new List<CheckerPiece>();
            MoveDict = new Dictionary<CheckerPiece, List<CheckerTile>>();
            SelectedPiece = null;
            
            
            for (int i = 0; i < AllPieces.Count; i++)
            {
                CheckAvailableMoves(AllPieces[i]);
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

            if (CheckValidTile(row + 1, col - 1))
            {
                targetPiece = GetCheckerPiece(row + 1, col - 1);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (CheckValidTile(row + 2, col - 2) && GetCheckerPiece(TileBoard[row + 2, col - 2]) == null)
                    {
                        tempPossibleMoves.Add(TileBoard[row + 2, col - 2]);
                    }
                }
            }
            if (CheckValidTile(row + 1, col + 1))
            {
                targetPiece = GetCheckerPiece(TileBoard[row + 1, col + 1]);
                if(targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (CheckValidTile(row + 2, col + 2) && GetCheckerPiece(TileBoard[row + 2, col + 2]) == null)
                    {
                        tempPossibleMoves.Add(TileBoard[row + 2, col + 2]);
                    }
                }
            }
            
            if (CheckValidTile(row - 1, col - 1))
            {
                targetPiece = GetCheckerPiece(TileBoard[row - 1, col - 1]);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (CheckValidTile(row - 2, col - 2) && GetCheckerPiece(TileBoard[row - 2, col - 2]) == null)
                    {
                        tempPossibleMoves.Add(TileBoard[row - 2, col - 2]);
                    }
                }
            }
            
            if (CheckValidTile(row - 1, col + 1))
            {
                targetPiece = GetCheckerPiece(TileBoard[row - 1, col + 1]);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (CheckValidTile(row - 2, col + 2) && GetCheckerPiece(TileBoard[row - 2, col + 2]) == null)
                    {
                        tempPossibleMoves.Add(TileBoard[row - 2, col + 2]);
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
                        if (CheckValidTile(row + 1, col - 1) && GetCheckerPiece(TileBoard[row + 1, col - 1]) == null)
                        {
                            tempPossibleMoves.Add(TileBoard[row + 1, col - 1]);
                        }
                        if (CheckValidTile(row + 1, col + 1) && GetCheckerPiece(TileBoard[row + 1, col + 1]) == null)
                        {
                            tempPossibleMoves.Add(TileBoard[row + 1, col + 1]);
                        }
                    }
                    //current turn is black
                    else
                    {
                        //At most there are two regular moves left or right diagonal

                        //Check if the tile is valid and has no piece on it
                        if (CheckValidTile(row - 1, col - 1) && GetCheckerPiece(TileBoard[row - 1, col - 1]) == null)
                        {
                            tempPossibleMoves.Add(TileBoard[row - 1, col - 1]);
                        }
                        if (CheckValidTile(row - 1, col + 1) && GetCheckerPiece(TileBoard[row - 1, col + 1]) == null)
                        {
                            tempPossibleMoves.Add(TileBoard[row - 1, col + 1]);
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
            if (mustJump[SelectedPiece.Color])
            {
                for (int i = 0; i < AllPieces.Count(); i++)
                {
                    if (AllPieces[i].Row == (newRow + SelectedPiece.Row) / 2 && AllPieces[i].Col == (newCol + SelectedPiece.Col) / 2)
                    {
                        AllPieces.RemoveAt(i);
                    }
                }
                jumpMade = SelectedPiece;
            }
            //Change the piece's position;
            SelectedPiece.Row = newRow;
            SelectedPiece.Col = newCol;

           
        }
        public int NextTurn(int currentTurn)
        {
            int nextTurn = currentTurn;
            if (jumpMade != null)
            {
                CheckAllAvailableMoves();
                //Console.WriteLine("A jump was made. Re-evaluating jump possibility for jumped piece.\n");
            }
            if (jumpMade == null || !MovablePieces[jumpMade.Color].Contains(GetCheckerPiece(jumpMade.Row, jumpMade.Col)))
            {
                mustJump[0] = false;
                mustJump[1] = false;
                jumpMade = null;
                nextTurn = (currentTurn + 1) % 2;
                //Console.WriteLine("Player {0}'s Turn.\n", currentTurn);
                CheckAllAvailableMoves();
            }
            SelectedPiece = null;
            return nextTurn;
        }

        public bool canMove(int playerColor)
        {
            return (MovablePieces[playerColor].Count() != 0);
        }
        public void Update(GameTime gameTime, Camera cam)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
    }
}
