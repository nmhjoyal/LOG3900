﻿using System;
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
    class partieJeuViewModel: Screen, INotifyPropertyChanged, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>,
                              IHandle<wordSelectedEvent>, IHandle<startTurnRoutineEvent>, IHandle<endTurnRoutineVMEvent>
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        private Editeur editeur = new Editeur();
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
        private BindableCollection<dynamic> _joueurs;

        public RoundInfos roundInfos { get; set; }
        public StrokeCollection Traits { get; set; }
        public Dictionary<Stroke, int> strokes { get; set; }
        public IselectWordCommand _selectWordCommand { get; set; }
        public DrawingAttributes AttributsDessin { get; set; } = new DrawingAttributes();

        public BindableCollection<dynamic> joueurs
        {
            get { return _joueurs; }
            set { _joueurs = value;
                NotifyOfPropertyChange(() => joueurs);
            }
        }

        public string OutilSelectionne
        {
            get { return editeur.OutilSelectionne; }
            set { //ProprieteModifiee(); 
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
            set {
                //ProprieteModifiee(); 
                NotifyOfPropertyChange(() => OutilSelectionne);
                }
        }

        public int TailleTrait
        {
            get { return editeur.TailleTrait; }
            set { editeur.TailleTrait = value; }
        }

        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }

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
            _joueurs = new BindableCollection<dynamic>();
            this.canDraw = false;
            // _roundDuration = 30;
            this.Traits = editeur.traits;
            this.strokes = new Dictionary<Stroke, int>();
            _timerContent = _roundDuration;
            _selectWordCommand = new selectWordCommand(events);
            fillAvatars();
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

            this._socketHandler.onDrawing(this.Traits, this.strokes);
            this.roundInfos = new RoundInfos("", 0);
            this._socketHandler.offWaitingRoom();
            this._socketHandler.onMatch(this.roundInfos);
        }

        /// <summary>
        /// Appelee lorsqu'une propriété de VueModele est modifiée.
        /// Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir de VueModèle.
        /// L'évènement qui contient le nom de la propriété modifiée sera attrapé par la vue qui pourra
        /// alors mettre à jour les composants concernés.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected virtual void ProprieteModifiee([CallerMemberName] string propertyName = null)
        {
            Console.WriteLine("PM de partieJeuVM");
            Console.WriteLine("PM de partieJeuVM outil:");
            Console.WriteLine(editeur.OutilSelectionne);
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Traite les évènements de modifications de propriétés qui ont été lancés à partir
        /// du modèle.
        /// </summary>
        /// <param name="sender">L'émetteur de l'évènement (le modèle)</param>
        /// <param name="e">Les paramètres de l'évènement. PropertyName est celui qui nous intéresse. 
        /// Il indique quelle propriété a été modifiée dans le modèle.</param>
        private void EditeurProprieteModifiee(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(sender.ToString());
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

        /// <summary>
        /// C'est ici qu'est défini la forme de la pointe, mais aussi sa taille (TailleTrait).
        /// Pourquoi deux caractéristiques se retrouvent définies dans une même méthode? Parce que pour créer une pointe 
        /// horizontale ou verticale, on utilise une pointe carrée et on joue avec les tailles pour avoir l'effet désiré.
        /// </summary>
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

        public int currentRound
        {
            get { return this.roundInfos.round; }
        }

        public string roundText
        {
            get { return currentRound + " of " + _userData.nbRounds; }
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
            _timer.Stop();
            this.roundInfos.round = this._userData.firstRound.currentRound;
            List<Score> scores = new List<Score>(this._userData.firstRound.scores);
            Console.WriteLine(scores.Count);
            this.newScores(scores);
            fillPlayers();
            dynamic endTurn = new System.Dynamic.ExpandoObject();
            endTurn.currentRound = this._userData.firstRound.currentRound;
            endTurn.drawer = this._userData.firstRound.drawer;
            this.canDraw = false;
            endTurn.nextIsYou = this._userData.firstRound.drawer == this._userData.userName;
            newWords(this._userData.firstRound.choices);
            newScores(this._userData.firstRound.scores);
            _events.PublishOnUIThread(new endTurnRoutineEvent(endTurn));
        }

        public void fillPlayers()
        {
            joueurs.Clear();
            List<Score> scores = new List<Score>(this._userData.firstRound.scores);
            foreach (Score score in scores)
            {
                dynamic player = new System.Dynamic.ExpandoObject();
                player.username = score.username;
                player.score = score.updateScore.scoreTotal;
                joueurs.Add(player);
            }
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
            Console.WriteLine("! start turn !");
            Console.WriteLine(message.turnTime);
            timerContent = message.turnTime;
            _timer.Start();
        }

        public void Handle(endTurnRoutineVMEvent message)
        {
            this.HandleFirstRound();
        }

        public void updateRoundInfos()
        {
            NotifyOfPropertyChange(null);
        }
    }
}
