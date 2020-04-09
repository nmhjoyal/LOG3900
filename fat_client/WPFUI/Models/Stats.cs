using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class StatsClient
    {
        public string username;
        public int matchCount;
        public double victoryPerc;
        public int averageTime;
        public int totalTime; 
        public int bestSSS;
        public int[] connections;
        public int[] disconnections;
        public MatchHistory[] matchesHistory; 
    }

    public class Stats
    {
        public string username;
        public int bestSSS;
        public int[] connections;
        public int[] disconnections;
        public MatchHistory[] matchesHistory;
    }
    public class MatchHistory
    {
        public int startTime;
        public int endTime;
        public string matchMode;
        public string[] playerNames;
        public Ranking winner;
        public int myScore;
    }
}
