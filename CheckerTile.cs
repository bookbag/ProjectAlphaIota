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
        public int Row { get; set; }
        public int Col { get; set; }
        public int Color { get; set; }
        public CheckerTile(int row, int col, int color = 0)
        {
            this.Row = row;
            this.Col = col;
            this.Color = color;
        }
       

    }
}
