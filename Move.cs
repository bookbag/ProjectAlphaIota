using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAlphaIota
{
    internal class Move
    {
        public int Row;
        public int Col;
        public Move(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    class MoveSet
    {
        public List<Move> MoveList = new List<Move>();

        public MoveSet(int row, int col)
        {
            MoveList.Add(new Move(row, col));
        }
        public MoveSet(MoveSet other)
        {
            MoveList = new List<Move>(other.MoveList);
        }
        public override string ToString()
        {
            return String.Format("{0}", String.Join(",", MoveList));
        
        }
    }

    class MoveResult
    {
        
    }
}
