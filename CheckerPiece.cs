using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAlphaIota
{
    class CheckerPiece
    {
        int color;

        public int Color
        {
            get { return color; }
            set { color = value; }
        }
        int row, col;
        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public int Col
        {
            get { return col; }
            set { col = value; }
        }
        public CheckerPiece(int row, int col, int color)
        {
            this.row = row;
            this.col = col;
            this.color = color;
        }
    }
}
