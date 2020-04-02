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

namespace WPFUI.ViewModels
{
    class partieJeuViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>,
                              IHandle<wordSelectedEvent>, IHandle<startTurnRoutineEvent>, IHandle<endTurnRoutineVMEvent>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Editeur editeur = new Editeur();

        public DrawingAttributes AttributsDessin { get; set; } = new DrawingAttributes();

        public string OutilSelectionne
        {
            get { return editeur.OutilSelectionne; }
            set { ProprieteModifiee(); }
        }

        public string CouleurSelectionnee
        {
            get { return editeur.CouleurSelectionnee; }
            set { editeur.CouleurSelectionnee = value; }
        }

        public string PointeSelectionnee
        {
            get { return editeur.PointeSelectionnee; }
            set { ProprieteModifiee(); }
        }

        public int TailleTrait
        {
            get { return editeur.TailleTrait; }
            set { editeur.TailleTrait = value; }
        }

        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private BindableCollection<Avatar> _avatars;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        public BindableCollection<dynamic> _wordChoices;
        public BindableCollection<dynamic> _turnScores;
        public int _timerContent;
        public DispatcherTimer _timer;
        private int _roundDuration;
        private string _guessBox;
        private bool canDraw;
        public RoundInfos roundInfos { get; set; }
        public StrokeCollection Traits { get; set; }
        public Dictionary<Stroke, int> strokes { get; set; }
        public IselectWordCommand _selectWordCommand { get; set; }

        public partieJeuViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);
            _events = events;
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            _userData = userdata;
            messages = userdata.messages;
            _timer = new DispatcherTimer();
            _wordChoices = new BindableCollection<dynamic>();
            _turnScores = new BindableCollection<dynamic>();
            this.canDraw = false;
            // _roundDuration = 30;
            this.Traits = editeur.traits;
            this.strokes = new Dictionary<Stroke, int>();
            _timerContent = _roundDuration;
            _selectWordCommand = new selectWordCommand(events);
            fillAvatars();
            startTimer();
            this.roundInfos = new RoundInfos("", 0);
            this._socketHandler.onMatch(this.roundInfos);
            this._socketHandler.onDrawing(this.Traits, this.strokes);
        }

        public void startTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
            _timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {
         
            if (timerContent != 0)
            {
                timerContent = _timerContent - 1;
            } else
            {
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

        public int currentRound
        {
            get { return this.roundInfos.round; }
        }

        public string currentWord
        {
            get { return this.roundInfos.word; }
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
            this.roundInfos.word = "stroke collected";
            if(!this.canDraw)
            {
                this.Traits.Remove(stroke);
            }
        }

        public BindableCollection<dynamic> turnScores
        {
            get { return _turnScores; }
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
            _events.PublishOnUIThread(new goBackMainEvent());
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

        public void newScores(List<Score> scores)
        {
            _turnScores.Clear();
            Console.WriteLine("!" + scores.Count);
            foreach (Score score in scores)
            {
                dynamic dynamicScore = new System.Dynamic.ExpandoObject();
                dynamicScore.position = 0;
                dynamicScore.name = score.username;
                dynamicScore.score = score.updateScore.scoreTotal;
                _turnScores.Add(dynamicScore);
            }
            turnScores.Refresh();
        }

        public void HandleFirstRound()
        {
            this.roundInfos.round = this._userData.firstRound.currentRound;
            List<Score> scores = new List<Score>(this._userData.firstRound.scores);
            Console.WriteLine(scores.Count);
            this.newScores(scores);

            dynamic endTurn = new System.Dynamic.ExpandoObject();
            endTurn.currentRound = this._userData.firstRound.currentRound;
            endTurn.drawer = this._userData.firstRound.drawer;
            this.canDraw = false;
            endTurn.nextIsYou = this._userData.firstRound.drawer == this._userData.userName;
            newWords(this._userData.firstRound.choices);
            newScores(this._userData.firstRound.scores);
            _events.PublishOnUIThread(new endTurnRoutineEvent(endTurn));
        }

        public void sendGuess()
        {
            if (guessBox != null & guessBox != "")
            {
                Console.WriteLine(guessBox);
                _socketHandler.socket.Emit("guess", guessBox);
            }
            guessBox = "";
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
                Console.WriteLine(this.OutilSelectionne);
                if (this.OutilSelectionne == "crayon")
                {
                    StylusPointCollection stylusPoint = new StylusPointCollection();
                    stylusPoint.Add(new StylusPoint(x, y));
                    Stroke stroke = new Stroke(stylusPoint);
                    stroke.DrawingAttributes.Width = this.AttributsDessin.Width;
                    stroke.DrawingAttributes.Height = this.AttributsDessin.Height;
                    stroke.DrawingAttributes.Color = this.AttributsDessin.Color;
                    Console.WriteLine("should emit");
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
            Console.WriteLine("!" + message.word);
            this.canDraw = true;
            /* TODO envoyer le mot au serveur */
            this.roundInfos.word = message.word;
            _socketHandler.socket.Emit("start_turn", message.word);
        }

        public void Handle(startTurnRoutineEvent message)
        {
            _timerContent = message.turnTime;
            _timer.Start();
        }

        public void Handle(endTurnRoutineVMEvent message)
        {
            this.HandleFirstRound();
        }

        protected virtual void ProprieteModifiee([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public void updateRoundInfos()
        {
            NotifyOfPropertyChange(null);
        }
    }
}
