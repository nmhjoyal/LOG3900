using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace WPFUI.Models
{
    class CreateGame
    {
        public string word;
        public StrokeCollection lines;
        public int level;
        public List<string> clues;
        public int mode;

        public CreateGame(string word, StrokeCollection lines, int level, List<string> clues, int mode)
        {
            this.word = word;
            this.lines = new StrokeCollection(lines);
            this.level = level;
            this.clues = new List<string>(clues);
            this.mode = mode;
        }
    }

}
