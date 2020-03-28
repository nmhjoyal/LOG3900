using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Match
    {
        public string matchId;
        public string host;
        public int nbRounds;
        public MatchMode matchMode;
        public List<PublicProfile> players;

        public Match(string matchId, string host, int nbRounds, MatchMode matchMode, List<PublicProfile> players)
        {
            this.matchId = matchId;
            this.host = host;
            this.nbRounds = nbRounds;
            this.matchMode = matchMode;
            this.players = new List<PublicProfile>(players);
        }

        public string MatchId
        {
            get
            {
                return this.matchId;
            }
        }
        public string Host
        {
            get
            {
                return this.host;
            }
        }
        public int NbRounds
        {
            get
            {
                return this.nbRounds;
            }
        }

        public string MatchMode
        {
            get
            {
                switch (this.matchMode)
                {
                    case Models.MatchMode.freeForAll:
                        return "Mêlée génétale";
                    case Models.MatchMode.oneVsOne:
                        return "1 vs 1";
                    case Models.MatchMode.sprintSolo:
                        return "Sprint solo";
                    case Models.MatchMode.sprintCoop:
                        return "Sprint coopératif";
                    case Models.MatchMode.inverted:
                        return "Mode inversé";
                    default:
                        return "";
                }
            }
        }

        public string Players
        {
            get
            {
                string players = "";
                foreach(PublicProfile player in this.players)
                {
                    players += player.username + " ";
                }
                return players;
            }
        }
    }

    public enum MatchMode
    {
        freeForAll,
        sprintSolo,
        sprintCoop,
        oneVsOne,
        inverted
    }

    public class PublicProfile
    {
        public string username;
        public string avatar;
    }

    public class CreateMatch
    {
        public int nbRounds;
        public int timeLimit;
        public MatchMode matchMode;

        public CreateMatch(int nbRounds, int timeLimit, MatchMode matchMode)
        {
            this.nbRounds = nbRounds;
            this.timeLimit = timeLimit;
            this.matchMode = matchMode;
        }
    }
}
