using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace WPFUI.Models
{
    public class Game
    {
        public string word;
        public StrokeCollection drawing;
        public List<string> clues;
        public Level level;
        public Mode mode;
        public int option;
        public Game(string word, StrokeCollection drawing, List<string> clues, Level level, Mode mode, int option)
        {
            this.word = word;
            this.drawing = new StrokeCollection(drawing);
            this.clues = new List<string>(clues);
            this.level = level;
            this.mode = mode;
            this.option = option;
        }
    }

    public class GamePreview
    {
        public StrokeCollection drawing;
        public Mode mode;
        public int option;
        public GamePreview(StrokeCollection drawing, Mode mode, int option)
        {
            this.drawing = new StrokeCollection(drawing);
            this.mode = mode;
            this.option = option;
        }
    }

    public enum Level
    {
        Easy,
        Medium,
        Hard
    }

    public enum Mode
    {
        Classic,
        Random,
        Panoramic,
        Centered
    }
}
