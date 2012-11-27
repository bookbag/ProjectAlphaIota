using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectAlphaIota
{
    class Board
    {
        public int TileScale { get; set; }
        public CheckerTile[,] TileBoard { get; set; }
        private int Rows { get; set; }
        private int Cols { get; set; }
        public Board(int rows, int cols, int tileScale)
        {
            Rows = rows;
            Cols = cols;

            TileScale = tileScale;

            TileBoard = new CheckerTile[rows, cols];
            
            bool black = true;
            for (var row = 0; row < rows; row++)    //top to down
            {
                for (int col = cols - 1; col >= 0; col--) // right to left
                {
                    TileBoard[row, col] = new CheckerTile(row, col, (black)?1:0);

                    if (col != 0) black = !black;
                }
            }
        }
        public bool CheckValidTile(int row, int col)
        {
            return row >= 0 && col >= 0 && row < Rows && col < Cols;
        }
        public CheckerTile GetCheckerTile(int row, int col)
        {
            if (CheckValidTile(row, col))
                return TileBoard[row, col];
            return null;
        }
        public CheckerTile GetCheckerTile(Vector2 position)
        {
            var row = (int)((position.Y) / TileScale);
            var col = (int)((position.X) / TileScale);
            return CheckValidTile(row, col) ? TileBoard[row, col] : null;
        }
       
    }
}
