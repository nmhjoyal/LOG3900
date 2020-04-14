using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFUI.EventModels;
using WPFUI.Models;
using WPFUI.Utilitaires;

namespace WPFUI.ViewModels
{

    
    /// <summary>
    /// Sert d'intermédiaire entre la vue et le modèle.
    /// Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    /// Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    class FenetreDessinViewModel : Screen, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Editeur editeur = new Editeur();
        public IUserData userdata;
        public IEventAggregator events;
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

        private StrokeCollection _traits;
        public StrokeCollection Traits
        {
            get { return _traits;  }
            set { _traits = value; ProprieteModifiee(); }
        }

        public Dictionary<Stroke, int> strokes;
        // Commandes sur lesquels la vue pourra se connecter.

        public RelayCommand<string> ChoisirPointe { get; set; }
        public RelayCommand<string> ChoisirOutil { get; set; }

        public RelayCommand<object> SendPoint { get; set; }


        /// <summary>
        /// Constructeur de VueModele
        /// On récupère certaines données initiales du modèle et on construit les commandes
        /// sur lesquelles la vue se connectera.
        /// </summary>
        private IEventAggregator _events;
        private IUserData _userdata;

        private ISocketHandler _socketHandler;

        private InkCanvas _canvas;

        public FenetreDessinViewModel(IEventAggregator events, ISocketHandler socketHandler, InkCanvas canvas)
        {
            _canvas = canvas;
            // SendPoint = new RelayCommand<object>(sendStrokeAction);
            this._socketHandler = socketHandler;
            _events = events;
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditeurProprieteModifiee est appelée.
            editeur.PropertyChanged += new PropertyChangedEventHandler(EditeurProprieteModifiee);

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            AttributsDessin = new DrawingAttributes();
            AttributsDessin.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(editeur.CouleurSelectionnee);
            AjusterPointe();

            Traits = editeur.traits;


            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            ChoisirPointe = new RelayCommand<string>(editeur.ChoisirPointe);
            ChoisirOutil = new RelayCommand<string>(editeur.ChoisirOutil);
            //_socketHandler.getStrokes(Canvas);
            this.strokes = new Dictionary<Stroke, int>();
            this._socketHandler.onDrawing(this.Traits, this.strokes);
            this._socketHandler.socket.Emit("connect_free_draw");
        }

        public void sendPoint(int x, int y)
        {
            StylusPoint stylusPoint = new StylusPoint(x, y);
            this._socketHandler.socket.Emit("point", JsonConvert.SerializeObject(stylusPoint));
        }

        public void sendStroke(int x, int y)
        {
            if(this.OutilSelectionne == "crayon")
            {
                StylusPointCollection stylusPoint = new StylusPointCollection();
                stylusPoint.Add(new StylusPoint(x, y));
                Stroke stroke = new Stroke(stylusPoint);
                stroke.DrawingAttributes.Width = this.AttributsDessin.Width;
                stroke.DrawingAttributes.Height = this.AttributsDessin.Height;
                stroke.DrawingAttributes.Color = this.AttributsDessin.Color;
                this._socketHandler.socket.Emit("stroke", JsonConvert.SerializeObject(stroke));
            } else if(this.OutilSelectionne == "efface_trait")
            {
                this._socketHandler.socket.Emit("erase_stroke");
            } else if(this.OutilSelectionne == "efface_segment")
            {
                this._socketHandler.socket.Emit("erase_point");
            }
        }

        public void getDrawing()
        {
            this._socketHandler.socket.Emit("get_drawing");
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
            this._socketHandler.offDrawing();
            _events.PublishOnUIThread(new goBackMainEvent());
        }



    }

}
