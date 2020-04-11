using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using Newtonsoft.Json;
using WPFUI.Commands;
using WPFUI.EventModels;
using WPFUI.Models;
using WPFUI.Utilitaires;

namespace WPFUI.ViewModels
{
    class partieJeuViewModel : Screen, INotifyPropertyChanged, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>,
                              IHandle<wordSelectedEvent>, IHandle<startTurnRoutineEvent>, IHandle<endTurnRoutineVMEvent>,
                              IHandle<guessResponseEvent>, IHandle<endMatchEvent>, IHandle<hintEvent>
    {
        private Editeur editeur = new Editeur();
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private BindableCollection<Avatar> _avatars;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        public BindableCollection<dynamic> _wordChoices;
        public BindableCollection<dynamic> _turnScores;
        public BindableCollection<dynamic> _matchScores;
        public int _timerContent;
        public DispatcherTimer _timer;
        private string _guessBox;
        private bool canDraw;
        private string _guessFeedBackSource;
        private string _guessFeedBackText;
        private string _winnerMessage;
        private bool hintEnabled;
        private int _countHelper;
        string now;

        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }

        public partieJeuViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            now = DateTime.Now.ToString();
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);
            _events = events;
            _events.Unsubscribe(this);
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            _userData = userdata;
            messages = userdata.messages;
            _guessFeedBackSource = "";
            _guessFeedBackText = "";
            _winnerMessage = "";
            _timer = new DispatcherTimer();
            _wordChoices = new BindableCollection<dynamic>();
            _turnScores = new BindableCollection<dynamic>();
            _matchScores = new BindableCollection<dynamic>();
            this.canDraw = false;
            // _roundDuration = 30;
            this.Traits = editeur.traits;
            this.strokes = new Dictionary<Stroke, int>();
            _timerContent = 0;
            _selectWordCommand = new selectWordCommand(events);
            // fillAvatars();
            startTimer();
            _timer.Stop();
            /* ----------------------------------- Drawing editor declarations -----------------------------------------------*/
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditeurProprieteModifiee est appelée.
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            AttributsDessin = new DrawingAttributes();
            AttributsDessin.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            AjusterPointe();

            Traits = editeur.traits;
            this.strokes = new Dictionary<Stroke, int>();

            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChoisirPointe = new RelayCommand<string>(editeur.ChoisirPointe);
            ChoisirOutil = new RelayCommand<string>(editeur.ChoisirOutil);

            this.HintEnabled = false;
            this._socketHandler.onDrawing(this.Traits, this.strokes);
            this.startTurn = new StartTurn("", 0);
            this.endTurn = new EndTurn(0, new List<string>(), "", new BindableCollection<Player>());
            this._socketHandler.offWaitingRoom();
            this._socketHandler.onMatch(this.startTurn, this.endTurn);
            _countHelper = 0;
        }

        public void Unsubscribe()
        {
            _events.Unsubscribe(this);
        }

        public string winnerMessage
        {
            get { return _winnerMessage; }
        }

        public StartTurn startTurn;

        public EndTurn endTurn;
        public StrokeCollection Traits { get; set; }
        public Dictionary<Stroke, int> strokes { get; set; }
        public IselectWordCommand _selectWordCommand { get; set; }
        public DrawingAttributes AttributsDessin { get; set; } = new DrawingAttributes();
        public string MatchMode
        {
            get
            {
                switch (this._userData.matchMode)
                {
                    case Models.MatchMode.freeForAll:
                        return "Free for all";
                    case Models.MatchMode.oneVsOne:
                        return "1 vs 1";
                    case Models.MatchMode.sprintSolo:
                        return "Solo sprint";
                    case Models.MatchMode.sprintCoop:
                        return "Coop sprint";
                    default: return "";
                }
            }
        }
        public string guessFeedBackSource
        {
            get { return _guessFeedBackSource; }
            set
            {
                _guessFeedBackSource = value;
                NotifyOfPropertyChange(() => guessFeedBackSource);
            }
        }

        public string guessFeedBackText
        {
            get { return _guessFeedBackText; }
            set
            {
                _guessFeedBackText = value;
                NotifyOfPropertyChange(() => guessFeedBackText);
            }
        }

        public bool HintEnabled
        {
            get { return this.hintEnabled; }
            set
            {
                this.hintEnabled = value;
                NotifyOfPropertyChange(() => HintEnabled);
            }
        }

        public BindableCollection<Player> joueurs
        {
            get { return this.endTurn.players; }
            set
            {
                this.endTurn.players = value;
                NotifyOfPropertyChange(() => joueurs);
            }
        }

        public string OutilSelectionne
        {
            get { return editeur.OutilSelectionne; }
            set
            { 
                NotifyOfPropertyChange(() => OutilSelectionne);
            }
        }

        public string CouleurSelectionnee
        {
            get { return editeur.CouleurSelectionnee; }
            set { editeur.CouleurSelectionnee = value; }
        }

        public string PointeSelectionnee
        {
            get { return editeur.PointeSelectionnee; }
            set
            {
                NotifyOfPropertyChange(() => OutilSelectionne);
            }
        }

        public int TailleTrait
        {
            get { return editeur.TailleTrait; }
            set { editeur.TailleTrait = value; }
        }

        private void EditeurProprieteModifiee(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CouleurSelectionnee")
            {
                AttributsDessin.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            }
            else if (e.PropertyName == "OutilSelectionne")
            {
                OutilSelectionne = editeur.OutilSelectionne;
            }
            else if (e.PropertyName == "PointeSelectionnee")
            {
                PointeSelectionnee = editeur.PointeSelectionnee;
                AjusterPointe();
            }
            else // e.PropertyName == "TailleTrait"
            {
                AjusterPointe();
            }
        }

        private void AjusterPointe()
        {
            AttributsDessin.StylusTip = (editeur.PointeSelectionnee == "ronde") ? StylusTip.Ellipse : StylusTip.Rectangle;
            AttributsDessin.Width = (editeur.PointeSelectionnee == "verticale") ? 1 : editeur.TailleTrait;
            AttributsDessin.Height = (editeur.PointeSelectionnee == "horizontale") ? 1 : editeur.TailleTrait;
        }

        public void startTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
            _timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {

            if (timerContent != 1)
            {
                timerContent = timerContent - 1;
            } else
            {
                timerContent = 0;
                _timer.Stop();
            }
        }

        public int timerContent
        {
            get { return _timerContent; }
            set { _timerContent = value;
                  NotifyOfPropertyChange(() => timerContent);
                }   
        }
        public string roundText
        {
            get { return this.endTurn.currentRound + " of " + _userData.nbRounds; }
        }

        public string currentWord
        {
            get { return this.startTurn.word; }
        }

        public string guessBox
        {
            get { return _guessBox; }
            set { _guessBox = value;
                NotifyOfPropertyChange(() => guessBox);
            }
        }

        public BindableCollection<dynamic> wordChoices
        {
            get { return _wordChoices; }
        }

        internal void strokeCollected(Stroke stroke)
        {
            if(!this.canDraw)
            {
                this.Traits.Remove(stroke);
            }
        }

        public BindableCollection<dynamic> turnScores
        {
            get { return _turnScores; }
        }

        public BindableCollection<dynamic> matchScores
        {
            get { return _matchScores; }
        }

        public IEventAggregator events
        {
            get { return _events; }
        }

        public string currentMessage
        {
            get { return _currentMessage; }
            set
            {
                _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);
                _userData.currentMessage = value;
            }
        }

        public BindableCollection<Models.Message> messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                NotifyOfPropertyChange(() => messages);
            }
        }
        public void keyDown(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                //sendMessage();
            }
        }
        public void goBack()
        {
            leaveMatchRoutine();
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void goToGameView()
        {
            leaveMatchRoutine();
            _events.PublishOnUIThread(new joinGameEvent());
        }

        public void leaveMatchRoutine()
        {
            _socketHandler.socket.Emit("leave_chat_room", _userData.matchId);
            _socketHandler.socket.Emit("leave_match");
            _socketHandler.offMatch();
            _userData.matchId = null;
            _userData.currentGameRoom = null;
            Room general = _userData.selectableJoinedRooms[0].room;
            _userData.messages = new BindableCollection<Models.Message>(general.messages);
            _userData.currentRoomId = general.roomName;
            _events.PublishOnUIThread(new refreshMessagesEvent(_userData.messages, _userData.currentRoomId));
        }

        public string username
        {
            get { return _userData.userName; }
        }

        public void fillAvatars()
        {
            _avatars = new BindableCollection<Avatar>();
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
        }
        public string avatarSource
        {
            get { return _avatars.Single(i => i.name == _userData.avatarName).source; }
        }

        public string getAvatarSource( string avatarType)
        {
            return _avatars.Single(i => i.name == avatarType).source;
        }

        public void sendMessage()
        {
            _socketHandler.sendMessage();
            currentMessage = "";
            _userData.currentMessage = "";
        }

        public void newWords(List<string> choices)
        {
            _wordChoices.Clear();
            foreach (string word in choices)
            {
                dynamic w = new System.Dynamic.ExpandoObject();
                w.word = word;
                _wordChoices.Add(w);
            }
            wordChoices.Refresh();
        }

        public void newScores()
        {
            _turnScores.Clear();
            foreach (Player player in this.endTurn.HumanPlayers)
            {
                dynamic dynamicScore = new System.Dynamic.ExpandoObject();
                dynamicScore.position = 0;
                dynamicScore.name = player.Username;
                dynamicScore.score = player.ScoreTurn;
                _turnScores.Add(dynamicScore);
            }

            _turnScores = new BindableCollection<dynamic>(_turnScores.OrderByDescending(i => i.score));

            int rank = 1;
            foreach (dynamic score in _turnScores)
            {
                score.position = rank + ".";
                score.score = ": +" + score.score + " points";
                rank++;
            }

            _turnScores = new BindableCollection<dynamic>(_turnScores.OrderBy(i => i.position));

            turnScores.Refresh();
            NotifyOfPropertyChange(() => turnScores);
        }
        public void sendGuess()
        {
            if (guessBox != null & guessBox != "")
            {
                _socketHandler.socket.Emit("guess", guessBox);
            }
            guessBox = "";
        }

        public void requestHint()
        {
            this.HintEnabled = false;
            this._socketHandler.socket.Emit("hint");
        }

        public void sendPoint(int x, int y)
        {
            if(this.canDraw)
            {
                StylusPoint stylusPoint = new StylusPoint(x, y);
                this._socketHandler.socket.Emit("point", JsonConvert.SerializeObject(stylusPoint));
            }
        }

        public void sendStroke(int x, int y)
        {
            if(this.canDraw)
            {
                if (this.OutilSelectionne == "crayon")
                {
                    StylusPointCollection stylusPoint = new StylusPointCollection();
                    stylusPoint.Add(new StylusPoint(x, y));
                    Stroke stroke = new Stroke(stylusPoint);
                    stroke.DrawingAttributes.Width = this.AttributsDessin.Width;
                    stroke.DrawingAttributes.Height = this.AttributsDessin.Height;
                    stroke.DrawingAttributes.Color = this.AttributsDessin.Color;
                    this._socketHandler.socket.Emit("stroke", JsonConvert.SerializeObject(stroke));
                }
                else if (this.OutilSelectionne == "efface_trait")
                {
                    this._socketHandler.socket.Emit("erase_stroke");
                }
                else if (this.OutilSelectionne == "efface_segment")
                {
                    this._socketHandler.socket.Emit("erase_point");
                }
            }
        }

        public void Handle(refreshMessagesEvent message)
        {
            this._messages = message._messages;
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(addMessageEvent message)
        {
            this._messages.Add(message.message);
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(wordSelectedEvent message)
        {
            /* TODO envoyer le mot au serveur */
            this.startTurn.word = message.word;
            _socketHandler.socket.Emit("start_turn", message.word);
        }

        public void Handle(startTurnRoutineEvent message)
        {
            this.HintEnabled = false;
            this.OutilSelectionne = "crayon";
            this.AttributsDessin.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFFFFF");
            this.AttributsDessin.Width = 1;
            this.AttributsDessin.Height = 1;
            if (this.endTurn.drawer == this._userData.userName)
            {
                this.AttributsDessin.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#000000");
                this.AttributsDessin.Width = 5;
                this.AttributsDessin.Height = 5;
                this.canDraw = true;
            }
            timerContent = message.turnTime;
            _timer.Start();
        }

        public void Handle(endTurnRoutineVMEvent message)
        {
            this.HintEnabled = false;
            _countHelper++;
            _timer.Stop();
            this.newScores();
            // fillPlayers();
            dynamic endTurn = new System.Dynamic.ExpandoObject();
            endTurn.currentRound = this.endTurn.currentRound;
            endTurn.drawer = this.endTurn.drawer;
            endTurn.nextIsYou = this.endTurn.drawer == this._userData.userName;
            this.canDraw = false;
            this.OutilSelectionne = "crayon";
            this.AttributsDessin.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#FFFFFF");
            this.AttributsDessin.Width = 1;
            this.AttributsDessin.Height = 1;
            this.newWords(this.endTurn.choices);
            _events.PublishOnUIThread(new endTurnRoutineEvent(endTurn));
        }

        public void updateRoundInfos()
        {
            NotifyOfPropertyChange(null);
        }

        public void Handle(guessResponseEvent message)
        {
            if (message._isGoodGuess)
            {
                guessFeedBackText = "Good Guess !";
                guessFeedBackSource = "/Resources/good guess.png";
            } else
            {
                guessFeedBackText = "Bad Guess !";
                guessFeedBackSource = "/Resources/bad guess.png";
            }
        }

        public void Handle(endMatchEvent message)
        {
            _matchScores.Clear();
            foreach (Player player in message.players)
            {
                dynamic dynamicScore = new System.Dynamic.ExpandoObject();
                dynamicScore.position = 0;
                dynamicScore.name = player.Username;
                dynamicScore.score = player.ScoreTotal;
                _matchScores.Add(dynamicScore);
            }

            _matchScores = new BindableCollection<dynamic>(_matchScores.OrderByDescending(i => i.score));

            int rank = 1;
            foreach (dynamic score in _matchScores)
            {
                score.position = rank + ".";
                score.score = ": " + score.score + " points";
                rank++;
            }

            _matchScores = new BindableCollection<dynamic>(_matchScores.OrderBy(i => i.position));

            endTurn.players = new BindableCollection<Player>();
            NotifyOfPropertyChange(() => joueurs);
            _winnerMessage = "The winner is " + _matchScores[0].name;
            NotifyOfPropertyChange(() => winnerMessage);
            matchScores.Refresh();
            NotifyOfPropertyChange(() => matchScores);
            this._timer.Stop();
        }

        public void Handle(hintEvent message)
        {
            this.HintEnabled = message.HintEnable;
        }
    }
}
