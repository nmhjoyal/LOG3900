using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Media;
using WPFUI.EventModels;
using WPFUI.Models;
using WPFUI.Utilitaires;

namespace WPFUI.ViewModels
{
    class CreationJeuAssiste1ViewModel : Caliburn.Micro.Screen, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Editeur editeur = new Editeur();

        // Ensemble d'attributs qui définissent l'apparence d'un trait.
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

        public StrokeCollection Traits { get; set; }

        public Dictionary<Stroke, int> strokes { get; set; }

        // Commandes sur lesquels la vue pourra se connecter.

        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }

        public IEventAggregator events
        {
            get { return this._events; }
        }
        /// <summary>
        /// Constructeur de VueModele
        /// On récupère certaines données initiales du modèle et on construit les commandes
        /// sur lesquelles la vue se connectera.
        /// </summary>
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        public CreationJeuAssiste1ViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {

            _socketHandler = socketHandler;
            _events = events;
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
            this._socketHandler.onPreview();
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public void fullScreenChat()
        {
            _events.PublishOnUIThread(new fullScreenChatEvent());
        }

        public void mainMenu()
        {
            this._socketHandler.socket.Emit("clear");
            this._socketHandler.offDrawing();
            this._socketHandler.offPreview();
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void goBack()
        {
            this._socketHandler.socket.Emit("clear");
            this._socketHandler.offDrawing();
            this._socketHandler.offPreview();
            _events.PublishOnUIThread(new goBackCreationMenuEvent());
        }

        public bool createGame(string word, List<string> clues, int level, int mode, int option, string fileName, int width, int height, int thickness, System.Windows.Media.Color color, bool dotted)
        {
            try
            {
                CreateGame game = new CreateGame(word, Potrace.Converter.exec(fileName, width, height, thickness, color, dotted), clues, (Level)level, (Mode)mode, option);
                Feedback feedback = JsonConvert.DeserializeObject<Feedback>(this._socketHandler.TestPOSTWebRequest(game, "/game/create").ToString());
                if(feedback.status)
                {
                    _events.PublishOnUIThread(new appSuccessEvent(feedback.log_message));
                    return true;
                } else
                {
                    _events.PublishOnUIThread(new appWarningEvent(feedback.log_message));
                    return false;
                }
            }
            catch (Exception)
            {
                _events.PublishOnUIThread(new appWarningEvent("This file provided is invalid (bmp, jpg, png)"));
                return false;
            }
        }

        public void preview(string fileName, int mode, int option, int width, int height, int thickness, System.Windows.Media.Color color, bool dotted)
        {
            try
            {
                GamePreview gamePreview = new GamePreview(Potrace.Converter.exec(fileName, width, height, thickness, color, dotted), (Mode)mode, option);
                this._socketHandler.socket.Emit("preview", JsonConvert.SerializeObject(gamePreview));
            }
            catch (Exception)
            {
                this._events.PublishOnUIThread(new previewDoneEvent());
                _events.PublishOnUIThread(new appWarningEvent("This file provided is invalid (bmp, jpg, png)"));
            }
        }
        public void preventDrawing(Stroke stroke)
        {
            this.Traits.Remove(stroke);
        }
    }
}
