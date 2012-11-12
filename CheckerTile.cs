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
    class CheckerTile
    {
        int color; //0 - white 1- red
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
        public int Color
        {
            get{return color;}
            set{color = value;}
        }
        public CheckerTile(int row, int col, int color = 0)
        {
            this.row = row;
            this.col = col;
            this.color = color;
        }
       

    }
}
