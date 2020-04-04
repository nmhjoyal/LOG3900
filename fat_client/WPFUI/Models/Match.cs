using Caliburn.Micro;
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

        public string Username
        {
            get
            {
                return this.username;
            }
        }
        public string Avatar
        {
            get
            {
                BindableCollection<Avatar> _avatars = new BindableCollection<Avatar>();
                _avatars.Add(new Avatar("/Resources/apple.png", "APPLE"));
                _avatars.Add(new Avatar("/Resources/avocado.png", "AVOCADO"));
                _avatars.Add(new Avatar("/Resources/banana.png", "BANANA"));
                _avatars.Add(new Avatar("/Resources/cherry.png", "CHERRY"));
                _avatars.Add(new Avatar("/Resources/grape.png", "GRAPE"));
                _avatars.Add(new Avatar("/Resources/kiwi.png", "KIWI"));
                _avatars.Add(new Avatar("/Resources/lemon.png", "LEMON"));
                _avatars.Add(new Avatar("/Resources/orange.png", "ORANGE"));
                _avatars.Add(new Avatar("/Resources/pear.png", "PEAR"));
                _avatars.Add(new Avatar("/Resources/pineapple.png", "PINEAPPLE"));
                _avatars.Add(new Avatar("/Resources/strawberry.png", "STRAWBERRY"));
                _avatars.Add(new Avatar("/Resources/watermelon.png", "WATERMELON"));
                Console.WriteLine("***" + this.avatar);
                return _avatars.Single(i => i.name == this.avatar).source;
            }
        }
    }

    public class Player
    {
        public PublicProfile user;
        public Boolean isHost;
        public Boolean isVirtual;
        public int score;

        public string Username
        {
            get
            {
                return user.Username;
            }
        }

        public string Avatar
        {
            get
            {
                return user.Avatar;
            }
        }
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

    public class EndTurn
    {
        public int currentRound;
        public List<string> choices;
        public string drawer;
        public List<UsernameUpdateScore> scores;

        public EndTurn(int currentRound, List<string> choices, string drawer, List<UsernameUpdateScore> scores)
        {
            this.currentRound = currentRound;
            this.choices = new List<string>(choices);
            this.drawer = drawer;
            this.scores = new List<UsernameUpdateScore>(scores);
        }
    }
    public class UsernameUpdateScore
    {
        public string username;
        public int scoreTotal;
        public int scoreTurn;

        public UsernameUpdateScore(string username, int scoreTotal, int scoreTurn)
        {
            this.username = username;
            this.scoreTotal = scoreTotal;
            this.scoreTurn = scoreTurn;
        }
    }
}
