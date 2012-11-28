using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace ProjectAlphaIota
{
    class CheckerState
    {
        public bool[] MustJump { get; set; }
        public CheckerPiece JumpMade { get; set; }
        public List<CheckerPiece> AllPieces { get; set; }
        public List<CheckerPiece>[] MovablePieces { get; set; }
        public Dictionary<CheckerPiece, List<CheckerTile>> MoveDict { get; set; }
        public CheckerPiece SelectedPiece { get; set; }
        public Dictionary<int, CheckerPiece> CheckerPieceMap { get; set; }

        public CheckerState(int rows, int cols)
        {
            MustJump = new[]{false, false};

            bool black = true;
            AllPieces = new List<CheckerPiece>();
            CheckerPieceMap = new Dictionary<int, CheckerPiece>();
            for (int row = 0; row < rows; row++)    //top to down
            {
                for (int col = cols - 1; col >= 0; col--) // right to left
                {
                    CheckerPieceMap[row*cols + col] = null;
                    if (black)
                    {
                        var numPieceRows = (rows - 2) / 2;
                        if (row < numPieceRows)
                        {
                            var tempPiece = new CheckerPiece(row, col, 0);
                            AllPieces.Add(tempPiece);
                            CheckerPieceMap[row * cols + col] = tempPiece;
                        }
                        else if (row >= rows - numPieceRows)
                        {
                            var tempPiece = new CheckerPiece(row, col, 1);
                            AllPieces.Add(tempPiece);
                            CheckerPieceMap[row * cols + col] = tempPiece;
                        }
                    }

                    black = !black;
                    if (col == 0)
                        black = !black;
                }
            }
        }

        public CheckerState(CheckerState other)
        {
            AllPieces = other.AllPieces;
            CheckerPieceMap = new Dictionary<int, CheckerPiece>();
            foreach (KeyValuePair<int, CheckerPiece> entry in other.CheckerPieceMap)
            {
                CheckerPieceMap[entry.Key] = entry.Value;
            }
            MustJump = new[] { other.MustJump[0], other.MustJump[1] };
            SelectedPiece = null;
            JumpMade = other.JumpMade;
            AllPieces = other.AllPieces;
        }

       
    }
}
