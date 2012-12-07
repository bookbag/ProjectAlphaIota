using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAlphaIota
{
    enum CheckerPieceStatus
    {
        Alive,
        Dying,
        Dead
    }
    class CheckerPiece
    {

        public int Color { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public CheckerPieceStatus Status { get; set; }
        public DateTime TimeDead;
        public CheckerPiece(int row, int col, int color, CheckerPieceStatus status = CheckerPieceStatus.Alive)
        {
            Row = row;
            Col = col;
            Color = color;
            Status = status;           
        }
        public void Kill(DateTime currentTime)
        {
            Status = CheckerPieceStatus.Dying;
            TimeDead = currentTime;
        }
    }
}
