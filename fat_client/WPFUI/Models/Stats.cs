using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class StatsClient
    {
        public StatsClient(string username, int matchCount, double victoryPerc,
            List<int> connections, List<int> disconnections, List<MatchHistory> matchesHistory,
            int averageTime, int totalTime, int bestSSS)
        {
            this.username = username;
            this.matchCount = matchCount;
            this.victoryPerc = victoryPerc;
            this.averageTime = averageTime;
            this.totalTime = totalTime;
            this.bestSSS = bestSSS;
            this.connections = new List<int>(connections);
            this.disconnections = new List<int>(disconnections);
            this.matchesHistory = new List<MatchHistory>(matchesHistory);

        }

        public string username;
        public int matchCount;
        public double victoryPerc;
        public int averageTime;
        public int totalTime;
        public int bestSSS;
        public List<int> connections;
        public List<int> disconnections;
        public List<MatchHistory> matchesHistory;
      
    }

    public class Stats
    {
        public Stats(string username, int bestSSS)
        {
            this.username = username;
            this.bestSSS = bestSSS;
            this.connections = new List<int>(connections);
            this.disconnections = new List<int>(disconnections);

        }


        public string username;
        public int bestSSS;
        public List<int> connections;
        public List<int> disconnections;
        public List<MatchHistory> matchesHistory;
    }
    public class MatchHistory
    {
        public MatchHistory(int startTime, int endTime, string matchMode, Rank winner, int myScore, List<string> playerNames
           )
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.matchMode = matchMode;
            this.playerNames = new List<string>(playerNames);
            this.winner = winner;
            this.myScore = myScore;

        }

        public int startTime;
        public int endTime;
        public string matchMode;
        public List<string> playerNames;
        public Rank winner;
        public int myScore;
        
    }
}
