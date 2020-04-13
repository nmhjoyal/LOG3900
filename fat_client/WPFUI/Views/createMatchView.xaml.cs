using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFUI.EventModels;
using WPFUI.Models;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour createMatchView.xaml
    /// </summary>
    public partial class createMatchView : UserControl
    {
        createMatchViewModel _viewModel;
        IEventAggregator _events;

        public createMatchView()
        {
           
            InitializeComponent();
        }

        private void createMatch(object sender, MouseButtonEventArgs e)
        {
            _viewModel = DataContext as createMatchViewModel;
            _events = (DataContext as createMatchViewModel).events;
            try
            {
                if ((e.ClickCount == 1))
                {
                    /*TODO: Traduire le contenu des comboBox, envoyer les bonnes valeurs */
                    MatchMode matchMode = (MatchMode)this.modecomboBox.SelectedIndex;
                    int nbRounds = int.Parse(this.roundcomboBox.Text);
                    int timeLimit = int.Parse(this.timecomboBox.Text.Substring(0, this.timecomboBox.Text.Length - 8));
                    (this.DataContext as createMatchViewModel).createMatch(matchMode, nbRounds, timeLimit);
                    this._events.PublishOnUIThread(new appSuccessEvent("Your match has been created!"));
                }
            }
            catch (Exception)
            {
                this._events.PublishOnUIThread(new appWarningEvent("Select an option from ALL the fields to create a match."));
            }

        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.modecomboBox.SelectedIndex == 1 || this.modecomboBox.SelectedIndex == 2)
            {
                this.roundcomboBox.SelectedIndex = 0;
                this.roundcomboBox.IsEnabled = false;
            } else
            {
                this.roundcomboBox.IsEnabled = true;
            }
        }
    }
}
