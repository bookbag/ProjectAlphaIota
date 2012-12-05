using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAlphaIota
{
    class CheckerPieces
    {
        private int Rows { get; set; }
        private int Cols { get; set; }
        private List<CheckerPiece> AllPieces { get; set; }
        private CheckerPiece[,] PieceLocation;
        public CheckerPieces(int rows, int cols)
        {
            var black = true;
            Rows = rows;
            Cols = cols;
            AllPieces = new List<CheckerPiece>();
            PieceLocation = new CheckerPiece[Rows, Cols];
            for (var row = 0; row < Rows; row++)    //top to down
            {
                for (var col = Cols - 1; col >= 0; col--) // right to left
                {
                    if (black)
                    {
                        var numPieceRows = Math.Floor((Rows - 2) / 2.0);
                        if (row < numPieceRows)
                        {
                            var tempPiece = new CheckerPiece(row, col, 0);
                            AllPieces.Add(tempPiece);

                            PieceLocation[row, col] = tempPiece;
                        }
                        else if (row >= Rows - numPieceRows)
                        {
                            var tempPiece = new CheckerPiece(row, col, 1);
                            AllPieces.Add(tempPiece);

                            PieceLocation[row, col] = tempPiece;
                        }
                    }

                    if (col != 0 || Cols % 2 != 0) black = !black;
                }
            }
        }
        public CheckerPieces(CheckerPieces other)
        {
            PieceLocation = new CheckerPiece[Rows, Cols];
            for(var row = 0; row < Rows; row++)
            {
                for(var col = 0; col< Cols; col++)
                {
                    PieceLocation[row, col] = other.PieceLocation[row, col];
                }
            }
        }
        public CheckerPiece GetPieceAt(int row, int col)
        {
            return PieceLocation[row, col];
        }
        public void RemovePiece(int row, int col)
        {
            PieceLocation[row, col] = null;
        }
        public void Move(CheckerPiece piece, int row, int col)
        {
            PieceLocation[row, col] = piece;
            PieceLocation[row, col] = null;
        }
        public void EvaluatePieces()
    }
}
