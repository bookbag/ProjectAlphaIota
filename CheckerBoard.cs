using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public Dictionary<CheckerPiece, List<MoveSet>> MoveDict { get; set; }
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
                AllPieces.Add(new CheckerPiece(other.AllPieces[i].Row, other.AllPieces[i].Col, other.AllPieces[i].Color, other.AllPieces[i].Status));
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
            int totalValue;
            int red = AllPieces.Count(piece => piece.Color == 0 && piece.Status == CheckerPieceStatus.Alive);
            int black = AllPieces.Count(piece => piece.Color == 1 && piece.Status == CheckerPieceStatus.Alive);
            if (player == 0)
                totalValue = 5 * (red - black);
            else
            {
                totalValue = 5 * (black - red);
            }
            /*
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
             * */
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

            //Count the number of pieces of a color
            int numWhite = AllPieces.Count(piece => piece.Status == CheckerPieceStatus.Alive && piece.Color == 0);

            //Win/Lose
            if (numWhite == AllPieces.Count(piece => piece.Status == CheckerPieceStatus.Alive))
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
            return AllPieces.Find(piece => piece.Status == CheckerPieceStatus.Alive && piece.Row == row && piece.Col == col );
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
            MoveDict = new Dictionary<CheckerPiece, List<MoveSet>>();
            SelectedPiece = null;            
            
            foreach (var t in AllPieces.Where(piece => piece.Status == CheckerPieceStatus.Alive))
            {
                CheckAvailableMoves(t);
            }
        }

        public List<MoveSet> CheckJumpMove(CheckerBoard currentBoard, CheckerPiece piece)
        {
            var row = piece.Row;
            var col = piece.Col;

            var jumpMoves = new List<MoveSet>();
            var possibleLocation = new List<Point>{new Point(1, -1), new Point(1, 1), new Point(-1, -1), new Point(-1, 1)};
            
            //Goes to each adjacent tile
            foreach(var location in possibleLocation)
            {
                //Get the adjacent piece that will be jumped
                var targetPiece = currentBoard.GetCheckerPiece(row + location.X, col + location.Y);

                //if the piece exists and the color is the opposite color
                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    //Check if there is a empty space after that piece and there is no piece at the location
                    if (Board.CheckValidTile(row + 2 * location.X, col + 2 * location.Y) && currentBoard.GetCheckerPiece(row + 2 * location.X, col + 2 * location.Y) == null)
                    {
                        var jump = new MoveSet(row + 2 * location.X, col + 2 * location.Y);
                        
                        //Check continuous jumps
                        //Generate a copy of the board

                        var nextBoard = new CheckerBoard(currentBoard);
                        nextBoard.SelectedPiece = nextBoard.GetCheckerPiece(piece.Row, piece.Col);
                        nextBoard.MustJump[piece.Color] = true;
                        //Move
                        nextBoard.HandleMove(row + 2 * location.X, col + 2 * location.Y);
                        
                        //Keep checking for jump moves
                        var nextJumps = CheckJumpMove(nextBoard, nextBoard.SelectedPiece);
                        if (nextJumps.Any())
                        {
                            foreach (var nextJump in nextJumps)
                            {
                                var branch = new MoveSet(jump);
                                branch.MoveList.AddRange(nextJump.MoveList);
                                jumpMoves.Add(branch);
                            }
                        }
                        else
                        {
                            jumpMoves.Add(jump);
                        }
                    }
                }
            }
            return jumpMoves;
        }
        public void CheckAvailableMoves(CheckerPiece piece)
        {
            var row = piece.Row;
            var col = piece.Col;

            var tempPossibleMoves = CheckJumpMove(this, piece);
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
                            tempPossibleMoves.Add(new MoveSet(row + 1, col - 1));
                        }
                        if (Board.CheckValidTile(row + 1, col + 1) && GetCheckerPiece(row + 1, col + 1) == null)
                        {
                            tempPossibleMoves.Add(new MoveSet(row + 1, col + 1));
                        }
                    }
                    //current turn is black
                    else
                    {
                        //At most there are two regular moves left or right diagonal

                        //Check if the tile is valid and has no piece on it
                        if (Board.CheckValidTile(row - 1, col - 1) && GetCheckerPiece(row - 1, col - 1) == null)
                        {
                            tempPossibleMoves.Add(new MoveSet(row - 1, col - 1));
                        }
                        if (Board.CheckValidTile(row - 1, col + 1) && GetCheckerPiece(row - 1, col + 1) == null)
                        {
                            tempPossibleMoves.Add(new MoveSet(row - 1, col + 1));
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
        /*
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
         */
        public void HandleMove(MoveSet moveSet, int timeDelay = 0)
        {
            
            foreach (var move in moveSet.MoveList)
            {
                if (MustJump[SelectedPiece.Color])
                {
                    //Remove the jumped piece

                    foreach (var piece in AllPieces.Where(piece => piece.Status == CheckerPieceStatus.Alive))
                    {
                        if (piece.Row == (move.Row + SelectedPiece.Row)/2 &&
                            piece.Col == (move.Col + SelectedPiece.Col) / 2)
                        {
                            piece.Kill(DateTime.Now);                
                            //AllPieces.RemoveAt(i);
                            break;
                        }
                    }
                }
                //Change the piece's position;
                SelectedPiece.Row = move.Row;
                SelectedPiece.Col = move.Col;
                if(timeDelay > 0 && move != moveSet.MoveList.Last())
                    Thread.Sleep(timeDelay);
            }
        }
        public void HandleMove(int newRow, int newCol)
        {
            //Remove the jumped piece
            if (MustJump[SelectedPiece.Color])
            {
                foreach (var piece in AllPieces.Where(piece => piece.Status == CheckerPieceStatus.Alive))
                {
                    if (piece.Row == (newRow + SelectedPiece.Row) / 2 && piece.Col == (newCol + SelectedPiece.Col) / 2)
                    {
                        piece.Kill(DateTime.Now);                
                        //AllPieces.RemoveAt(i);
                        break;
                    }
                }
                JumpMade = SelectedPiece;
            }
            //Change the piece's position;
            SelectedPiece.Row = newRow;
            SelectedPiece.Col = newCol;

           
        }
        public int AINextTurn(int currentTurn)
        {
            //There was a jump move made, need to see if there is possible after jumps

            MustJump[0] = false;
            MustJump[1] = false;
            JumpMade = null;
            int nextTurn = (currentTurn + 1) % 2;
            

            CheckAllAvailableMoves();
            SelectedPiece = null;
            return nextTurn;
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
            foreach(var piece in AllPieces)
            {
                if(piece.Status == CheckerPieceStatus.Dying)
                {
                    if((DateTime.Now - piece.TimeDead).TotalSeconds > 1 )
                        piece.Status = CheckerPieceStatus.Dead;
                    
                }

            }
        }
        
    }
}
