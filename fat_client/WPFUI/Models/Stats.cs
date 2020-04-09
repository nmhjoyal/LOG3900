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
            int averageTime, int totalTime, int bestSSS)
        {
            this._username = username;
            this._matchCount = matchCount;
            this._victoryPerc = victoryPerc;
            this._averageTime = averageTime;
            this._totalTime = totalTime;
            this._bestSSS = bestSSS;
            this._connections= new int[30];
            this._disconnections = new int[30];
            this._matchesHistory = new MatchHistory[10];

        }

        private string _username;

        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        private int _matchCount;

        public int matchCount
        {
            get { return _matchCount; }
            set { _matchCount = value; }
        }

        private double _victoryPerc;

        public double victoryPerc
        {
            get { return _victoryPerc; }
            set { _victoryPerc = value; }
        }

        private int _averageTime;

        public int averageTime
        {
            get { return _averageTime; }
            set { _averageTime = value; }
        }

        private int _totalTime;

        public int totalTime
        {
            get { return _totalTime; }
            set { _totalTime = value; }
        }
        private int _bestSSS;

        public int bestSSS
        {
            get { return _bestSSS; }
            set { _bestSSS = value; }
        }

        private int[] _connections;

        public int[] connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        private int[] _disconnections;

        public int[] disconnections
        {
            get { return _disconnections; }
            set { _disconnections = value; }
        }

        private MatchHistory[]  _matchesHistory;

        public MatchHistory[]  matchesHistory
        {
            get { return _matchesHistory; }
            set { _matchesHistory = value; }
        }


    }

    public class Stats
    {
        public Stats(string username, int bestSSS)
        {
            this._username = username;
            this._bestSSS = bestSSS;
            this._connections = new int[30];
            this._disconnections = new int[30];
            this._matchesHistory = new MatchHistory[10];

        }
        private string _username;

        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        private int _bestSSS;

        public int bestSSS
        {
            get { return _bestSSS; }
            set { _bestSSS = value; }
        }

        private int[] _connections;

        public int[] connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        private int[] _disconnections;

        public int[] disconnections
        {
            get { return _disconnections; }
            set { _disconnections = value; }
        }

        private MatchHistory[] _matchesHistory;

        public MatchHistory[] matchesHistory
        {
            get { return _matchesHistory; }
            set { _matchesHistory = value; }
        }
    }
    public class MatchHistory
    {
        public MatchHistory(int startTime, int endTime, string matchMode, Rank winner, int myScore
           )
        {
            this._startTime = startTime;
            this._endTime = endTime;
            this._matchMode = matchMode;
            this.playerNames = new string[8];
            this._winner = winner;
            this._myScore = myScore;

        }
        private int _startTime;

        public int startTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        private int _endTime;

        public int endTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        private string _matchMode;

        public string matchMode
        {
            get { return _matchMode; }
            set { _matchMode = value; }
        }

        private string[] _playerNames;

        public string[] playerNames
        {
            get { return _playerNames; }
            set { _playerNames = value; }
        }

        private Rank _winner;

        public Rank winner
        {
            get { return _winner; }
            set { _winner = value; }
        }

        private int _myScore;

        public int myScore
        {
            get { return _myScore; }
            set { _myScore = value; }
        }

    
    }
}
