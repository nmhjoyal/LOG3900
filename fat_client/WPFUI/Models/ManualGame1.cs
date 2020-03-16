using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    class ManualGame1
    {
        public string wordToGuess;
        public string[] clues;
        public int difficultyLevel;
        public ManualGame1(string word, string[] clues, int difficulty)
        {
            this.wordToGuess = word;
            this.clues = clues;
            this.difficultyLevel = difficulty;
        }
    }
    
}
