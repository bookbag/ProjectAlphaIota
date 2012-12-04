using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProjectAlphaIota
{
    class CheckerBoard
    {
        private int Rows { get; set; }
        private int Cols { get; set; }
        private int TileScale { get; set; }

        public Board Board;
        //Checker Piece variables
        bool[] MustJump { get; set; }
        public CheckerPiece JumpMade { get; set; }

        public List<CheckerPiece> AllPieces { get; set; }
        public List<CheckerPiece>[] MovablePieces { get; set; }
        public Dictionary<CheckerPiece, List<CheckerTile>> MoveDict { get; set; }
        public CheckerPiece SelectedPiece { get; set; }
        //public Dictionary<Point, CheckerPiece> CheckerPieceMap { get; set; }

        //copy constructor
        public CheckerBoard(CheckerBoard other)
        {
            Board = other.Board;
            MustJump = new[] { other.MustJump[0], other.MustJump[1] };
            SelectedPiece = null;
            JumpMade = null;
            AllPieces = new List<CheckerPiece>(other.AllPieces.Count());
            for (var i = 0; i < other.AllPieces.Count(); i++)
            {
                AllPieces.Add(new CheckerPiece(other.AllPieces[i].Row, other.AllPieces[i].Col, other.AllPieces[i].Color));
                if (other.JumpMade == null) continue;
                if (other.AllPieces[i] == other.JumpMade)
                {
                    JumpMade = AllPieces[i];
                }
            }            
        }
        public CheckerBoard(int rows, int cols, int tileScale = 80)
        {
            Rows = rows;
            Cols = cols;
            TileScale = tileScale;
            Board = new Board(rows, cols, tileScale);
            Reset(rows, cols);
        }        


        public int EvaluateBoard(int player)
        {
            var totalValue = 0;
            foreach (var piece in AllPieces)
            {
                var row = piece.Row + 1;
                if (player == 1)
                {
                    row = (Rows) - row;
                }
                var value = (Rows) - row;
                if (piece.Color != player)
                {
                    value *= -1;
                }

                totalValue += value;
            }
            return totalValue;
        }

        //checks the status of a checkerboard state, 0 = draw, -1 = lose, 1 = win, 2 = continue
        public CheckerStatus GetStatus(int playerColor)
        {
            //Game is a Draw
            if (MovablePieces[playerColor].Count == 0)
            {
                return CheckerStatus.Lose;
            }
            int numWhite = 0;
            //Count the number of pieces of a color
            for(int i = 0; i < AllPieces.Count(); i++)
            {
                if (AllPieces[i].Color == 0)
                    numWhite++;
            }

            //Win/Lose
            if (numWhite == AllPieces.Count() )
            {
                return playerColor == 0 ? CheckerStatus.Win : CheckerStatus.Lose;
            }
            if (numWhite == 0 )
            {
                return playerColor == 1 ? CheckerStatus.Win : CheckerStatus.Lose;
            }
            return CheckerStatus.Continue; 
            
        }
        public void Reset(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            //Place the pieces
            bool black = true;
            Board = new Board(rows, cols, TileScale);
            AllPieces = new List<CheckerPiece>();

            for (int row = 0; row < Rows; row++)    //top to down
            {
                for (int col = Cols - 1; col >= 0; col--) // right to left
                {
                    if (black)
                    {
                        var numPieceRows = Math.Floor((Rows - 2) / 2.0);
                        if (row < numPieceRows)
                        {
                            var tempPiece = new CheckerPiece(row, col, 0);
                            AllPieces.Add(tempPiece);
                        }
                        else if (row >= Rows - numPieceRows)
                        {
                            var tempPiece = new CheckerPiece(row, col, 1);
                            AllPieces.Add(tempPiece);
                        }
                    }

                    if (col != 0 || Cols % 2 != 0) black = !black;
                }
            }
            MustJump = new[]{false,false};
            CheckAllAvailableMoves();
        }
        
        public CheckerPiece GetCheckerPiece(int row, int col)
        {
            return AllPieces.FirstOrDefault(piece => piece.Row == row && piece.Col == col);
        }
        
        public CheckerPiece GetCheckerPiece(Vector2 position)
        {
            var row = (int)((position.Y) / Board.TileScale);
            var col = (int)((position.X) / Board.TileScale);
            if (Board.CheckValidTile(row, col))
            {
                return GetCheckerPiece(row, col);
            }
            return null;
        }
        public CheckerPiece GetCheckerPiece(CheckerTile tile)
        {
            return GetCheckerPiece(tile.Row, tile.Col);
        }
        
        public Vector2 GetCenterOfTile(int row, int col)
        {
            return new Vector2(col * Board.TileScale + .5f * Board.TileScale, row * Board.TileScale + .5f * Board.TileScale);
        }
        public void CheckAllAvailableMoves()
        {
            
            MovablePieces = new List<CheckerPiece>[2];
            MovablePieces[0] = new List<CheckerPiece>();
            MovablePieces[1] = new List<CheckerPiece>();
            MoveDict = new Dictionary<CheckerPiece, List<CheckerTile>>();
            SelectedPiece = null;            
            
            foreach (CheckerPiece t in AllPieces)
            {
                CheckAvailableMoves(t);
            }
        }
        public void CheckAvailableMoves(CheckerPiece piece)
        {
            int row = piece.Row;
            int col = piece.Col;
            var tempPossibleMoves = new List<CheckerTile>();
            CheckerPiece targetPiece;
            //Check if there is a jump move
            //At most there are four jump moves with a piece in between and a empty piece after
            //Check if the tile is valid and has a piece on it and the color is not yours

            if (Board.CheckValidTile(row + 1, col - 1))
            {
                targetPiece = GetCheckerPiece(row + 1, col - 1);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (Board.CheckValidTile(row + 2, col - 2) && GetCheckerPiece(row + 2, col - 2) == null)
                    {
                        tempPossibleMoves.Add(Board.TileBoard[row + 2, col - 2]);
                    }
                }
            }
            if (Board.CheckValidTile(row + 1, col + 1))
            {
                targetPiece = GetCheckerPiece(row + 1, col + 1);
                if(targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (Board.CheckValidTile(row + 2, col + 2) && GetCheckerPiece(row + 2, col + 2) == null)
                    {
                        tempPossibleMoves.Add(Board.TileBoard[row + 2, col + 2]);
                    }
                }
            }

            if (Board.CheckValidTile(row - 1, col - 1))
            {
                targetPiece = GetCheckerPiece(row - 1, col - 1);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (Board.CheckValidTile(row - 2, col - 2) && GetCheckerPiece(Board.TileBoard[row - 2, col - 2]) == null)
                    {
                        tempPossibleMoves.Add(Board.TileBoard[row - 2, col - 2]);
                    }
                }
            }

            if (Board.CheckValidTile(row - 1, col + 1))
            {
                targetPiece = GetCheckerPiece(row - 1, col + 1);
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece
                    if (Board.CheckValidTile(row - 2, col + 2) && GetCheckerPiece(row - 2, col + 2) == null)
                    {
                        tempPossibleMoves.Add(Board.TileBoard[row - 2, col + 2]);
                    }
                }
            }
        
            //regular move
            //Check for regular move if there is no jump moves
            if (!tempPossibleMoves.Any())
            {
                if (!MustJump[piece.Color])
                {
                    //If the current turn is white
                    if (piece.Color == 0)
                    {
                        //At most there are two regular moves left or right diagonal

                        //Check if the tile is valid and has no piece on it
                        if (Board.CheckValidTile(row + 1, col - 1) && GetCheckerPiece(row + 1, col - 1) == null)
                        {
                            tempPossibleMoves.Add(Board.TileBoard[row + 1, col - 1]);
                        }
                        if (Board.CheckValidTile(row + 1, col + 1) && GetCheckerPiece(row + 1, col + 1) == null)
                        {
                            tempPossibleMoves.Add(Board.TileBoard[row + 1, col + 1]);
                        }
                    }
                    //current turn is black
                    else
                    {
                        //At most there are two regular moves left or right diagonal

                        //Check if the tile is valid and has no piece on it
                        if (Board.CheckValidTile(row - 1, col - 1) && GetCheckerPiece(row - 1, col - 1) == null)
                        {
                            tempPossibleMoves.Add(Board.TileBoard[row - 1, col - 1]);
                        }
                        if (Board.CheckValidTile(row - 1, col + 1) && GetCheckerPiece(row - 1, col + 1) == null)
                        {
                            tempPossibleMoves.Add(Board.TileBoard[row - 1, col + 1]);
                        }
                    }
                }
            }
            //A jump can be made
            else
            {
                //A previous jump was made
                if (JumpMade != null)
                {
                    if (JumpMade == piece)
                    {
                        MovablePieces[piece.Color].Add(piece);
                        MustJump[piece.Color] = true;
                    }
                }
                //If there was no previous jump made
                else
                {
                    //If this is the first jump
                    if (MustJump[piece.Color] == false)
                    {
                        //Clear the movable piece set
                        MovablePieces[piece.Color].Clear();
                        MustJump[piece.Color] = true;
                    }
                    //if this piece is a jump piece add it
                    if (MustJump[piece.Color])
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
                if (!MustJump[piece.Color])
                    MovablePieces[piece.Color].Add(piece);
            }
        }
        public void HandleMove(int newRow, int newCol)
        {
            //Remove the jumped piece
            if (MustJump[SelectedPiece.Color])
            {
                for (int i = 0; i < AllPieces.Count(); i++)
                {
                    if (AllPieces[i].Row == (newRow + SelectedPiece.Row) / 2 && AllPieces[i].Col == (newCol + SelectedPiece.Col) / 2)
                    {
                        AllPieces.RemoveAt(i);
                    }
                }
                JumpMade = SelectedPiece;
            }
            //Change the piece's position;
            SelectedPiece.Row = newRow;
            SelectedPiece.Col = newCol;

           
        }
        public int NextTurn(int currentTurn)
        {
            int nextTurn = currentTurn;
            //There was a jump move made, need to see if there is possible after jumps
            if(JumpMade != null)
            {
                CheckAllAvailableMoves();
            }
            if (JumpMade == null || !MovablePieces[JumpMade.Color].Contains(GetCheckerPiece(JumpMade.Row, JumpMade.Col)))
            {
                MustJump[0] = false;
                MustJump[1] = false;
                JumpMade = null;
                nextTurn = (currentTurn + 1) % 2;
            }

            CheckAllAvailableMoves();
            SelectedPiece = null;
            return nextTurn;
        }

        public bool CanMove(int playerColor)
        {
            return (MovablePieces[playerColor].Count() != 0);
        }
        public void Update(GameTime gameTime, Camera cam)
        {
        }
        
    }
}
