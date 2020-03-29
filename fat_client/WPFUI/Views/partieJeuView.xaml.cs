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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFUI.EventModels;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour partieJeuView.xaml
    /// </summary>
    public partial class partieJeuView : UserControl, IHandle<endTurnRoutineEvent>
    {
        private partieJeuViewModel _viewModel;
        
        public partieJeuView()
        {
            InitializeComponent();
        }

        
        private void refocus(object sender, RoutedEventArgs e)
        {
            currentMessage.Focus();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as partieJeuViewModel;
            (DataContext as partieJeuViewModel).events.Subscribe(this);
        }

        private void currentMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //  _viewModel.sendMessage(currentMessage.Text);
            }
        }

        public async void Handle(endTurnRoutineEvent message)
        {
            if (((dynamic)message.EndTurnFeedBack).nextIsYou)
            {
                roundFinishedMessage.Text = "Round " + ((dynamic)message.EndTurnFeedBack).currentRound + " finished !";
                nextPlayerMessage.Text = "Next player to chose is " + ((dynamic)message.EndTurnFeedBack).drawer;
                endTurnBox.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                endTurnBox.Visibility = Visibility.Hidden;
                selectNextDrawingBox.Visibility = Visibility.Visible;
                await Task.Delay(4000);
                selectNextDrawingBox.Visibility = Visibility.Hidden;

            } else
            {
                endTurnBox.Visibility = Visibility.Visible;
                await Task.Delay(7000);
                endTurnBox.Visibility = Visibility.Hidden;
            }

        }

    }
}
