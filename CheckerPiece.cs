using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAlphaIota
{
    class CheckerPiece
    {

        public int Color { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public CheckerPiece(int row, int col, int color)
        {
            Row = row;
            Col = col;
            Color = color;
        }
    }
}
