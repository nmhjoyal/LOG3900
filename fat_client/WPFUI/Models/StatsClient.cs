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

        //methode prise de: https://ourcodeworld.com/articles/read/865/how-to-convert-an-unixtime-to-datetime-class-and-viceversa-in-c-sharp
        public  BindableCollection<DateTime> UnixTimeToDateTime(BindableCollection<long> unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            BindableCollection<DateTime> tab = new BindableCollection<DateTime>();
            foreach (long time in unixtime)
            {
                dtDateTime = dtDateTime.AddMilliseconds(time).ToLocalTime();
                tab.Add(dtDateTime);
                dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            }
            
            return tab;
        }
        public string Username {
            get { return this.username; }
        }

        public int MatchCount
        {
            get { return this.matchCount; }
        }

        public string VictoryPerc
        {
            get { return this.victoryPerc + "%"; }
        }

        public string AverageTime
        {
            get { return this.averageTime + " seconds"; }
        }

        public string TotalTime
        {
            get { return this.totalTime + " seconds"; }
        }

        public string BestSSS
        {
            get { return this.bestSSS + " points"; }
        }
        public BindableCollection<DateTime> Connections
        {
            get {

                return UnixTimeToDateTime(this.connections); }
        }

        public BindableCollection<DateTime> Disconnections
        {
            get { return UnixTimeToDateTime(this.disconnections); }
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

        //methode prise de: https://ourcodeworld.com/articles/read/865/how-to-convert-an-unixtime-to-datetime-class-and-viceversa-in-c-sharp
        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
        public DateTime StartTime
        {
            get { return UnixTimeToDateTime(this.startTime); }
           
        }

        public DateTime EndTime
        {
            get { return UnixTimeToDateTime(this.endTime); }

        }
        public string MatchMode
        {
            get 
            { 
                switch(this.matchMode)
                {
                    case Models.MatchMode.freeForAll:
                        return "Free for all";
                    case Models.MatchMode.sprintSolo:
                        return "Sprint solo";
                    case Models.MatchMode.sprintCoop:
                        return "Sprint coop";
                    case Models.MatchMode.oneVsOne:
                        return "1 vs 1";
                    default:
                        return "";
                }
            }

        }

        public BindableCollection<string> PlayerNames
        {
            get { return this.playerNames; }

        }

        public string Winner
        {
            get { return this.winner.username; }

        }
        public int MyScore
        {
            get { return this.myScore; }

        }
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
