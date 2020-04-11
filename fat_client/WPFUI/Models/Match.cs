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
    }

    public class PublicProfile
    {
        public string username;
        public string avatar;

        public PublicProfile(string username, string avatar)
        {
            this.username = username;
            this.avatar = avatar;
        }

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
                return _avatars.Single(i => i.name == this.avatar).source;
            }
        }
    }

    public class Player
    {
        public PublicProfile user;
        public bool isVirtual;
        public UpdateScore score;

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

        public string ScoreTotal
        {
            get
            {
                return isVirtual? "" : score.scoreTotal.ToString();
            }

        }
        public string ScoreTurn
        {
            get
            {
                return isVirtual ? "" : score.scoreTurn.ToString();
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
        public BindableCollection<Player> players;

        public BindableCollection<Player> HumanPlayers
        {
            get { return new BindableCollection<Player>(this.players.Where(player => !player.isVirtual)); }
        }
        public EndTurn(int currentRound, List<string> choices, string drawer, BindableCollection<Player> players)
        {
            this.currentRound = currentRound;
            this.choices = new List<string>(choices);
            this.drawer = drawer;
            this.players = new BindableCollection<Player>(players);
        }

        public void set(EndTurn endTurn)
        {
            this.currentRound = endTurn.currentRound;
            this.choices = endTurn.choices;
            this.drawer = endTurn.drawer;
            this.players = new BindableCollection<Player>(endTurn.players.OrderByDescending(i => i.ScoreTotal));
        }
    }
    public class UpdateScore
    {
        public int scoreTotal;
        public int scoreTurn;

        public UpdateScore(int scoreTotal, int scoreTurn)
        {
            this.scoreTotal = scoreTotal;
            this.scoreTurn = scoreTurn;
        }
    }

    public class StartTurn
    {
        public string word;
        public int timeLimit;

        public StartTurn(string word, int timeLimit)
        {
            this.word = word;
            this.timeLimit = timeLimit;
        }

        public void set(StartTurn startTurn, bool isDrawer)
        {
            if(!isDrawer)
            {
                this.word = string.Concat(startTurn.word.Select(letter => letter + " "));
            }
            this.timeLimit = startTurn.timeLimit;
        }
    }

    public class UpdateSprint
    {
        public int guess;
        public int time;
        public string word;
        public BindableCollection<Player> players;
        public BindableCollection<Player> HumanPlayers
        {
            get { return new BindableCollection<Player>(this.players.Where(player => !player.isVirtual)); }
        }
        public UpdateSprint(int guess, int time, string word, BindableCollection<Player> players)
        {
            this.guess = guess;
            this.time = time;
            this.word = word;
            this.players = new BindableCollection<Player>(players);
        }

        public void set(UpdateSprint updateSprint)
        {
            this.guess = updateSprint.guess;
            this.time = updateSprint.time;
            this.word = updateSprint.word;
            this.players = new BindableCollection<Player>(updateSprint.players.OrderByDescending(i => i.ScoreTotal));
        }
    }
}
