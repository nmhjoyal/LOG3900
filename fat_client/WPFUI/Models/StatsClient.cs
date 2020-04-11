﻿using Caliburn.Micro;
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
        public int victoryPerc;
        public int averageTime;
        public int totalTime;
        public int bestSSS;
        public BindableCollection<long> connections;
        public BindableCollection<long> disconnections;
        public BindableCollection<MatchHistory> matchesHistory;

        public string Username {
            get { return this.username; }
        }

        public int MatchCount
        {
            get { return this.matchCount; }
        }

        public int VictoryPerc
        {
            get { return this.victoryPerc; }
        }

        public int AverageTime
        {
            get { return this.averageTime; }
        }

        public int TotalTime
        {
            get { return this.totalTime; }
        }

        public int BestSSS
        {
            get { return this.bestSSS; }
        }
        public BindableCollection<long> Connections
        {
            get { 
                
                return this.connections; }
        }
        public BindableCollection<long> Disconnections
        {
            get { return this.disconnections; }
        }
        public BindableCollection<MatchHistory> MatchesHistory
        {
            get { return this.matchesHistory; }
        }
        public StatsClient(string username, int matchCount, int victoryPerc, int averageTime, int totalTime, int bestSSS, BindableCollection<long> connections, BindableCollection<long> disconnections, BindableCollection<MatchHistory> matchesHistory)
        {
            this.username = username;
            this.matchCount = matchCount;
            this.victoryPerc = victoryPerc;
            this.averageTime = averageTime;
            this.totalTime = totalTime;
            this.bestSSS = bestSSS;
            this.connections = new BindableCollection<long>(connections);
            this.disconnections = new BindableCollection<long>(disconnections);
            this.matchesHistory = new BindableCollection<MatchHistory>(matchesHistory);
        }

    }
    public class MatchHistory
    {
        public long startTime;
        public long endTime;
        public MatchMode matchMode;
        public BindableCollection<string> playerNames;
        public Rank winner;
        public int myScore;

        public MatchHistory(long startTime, long endTime, MatchMode matchMode, BindableCollection<string> playerNames, Rank winner, int myScore)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.matchMode = matchMode;
            this.playerNames = new BindableCollection<string>(playerNames);
            this.winner = winner;
            this.myScore = myScore;
        }
    
    }

    public class Rank
    {

        public string username;
        public int score;

        public Rank(string username, int score)
        {
            this.username = username;
            this.score = score;
        }
    }
}
