using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDFExtractor
{
    public class RepeatedWord
    {
        public string wordValue;
        public int[] locs;

        int index;

        public RepeatedWord(string wordVal, int[] loc)
        {
            this.wordValue = wordVal;
            this.locs = loc;

            index = -1;
        }

        public int Loc
        {
            get { ++index; return this.locs[index]; } 
        }
    }
}
