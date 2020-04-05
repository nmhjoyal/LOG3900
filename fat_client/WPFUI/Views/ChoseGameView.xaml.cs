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
using WPFUI.Models;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour ChoseGameView.xaml
    /// </summary>
    public partial class ChoseGameView : UserControl
    {
        public ChoseGameView()
        {
            InitializeComponent();
        }

        private void joinGame(object sender, RoutedEventArgs e)
        {
            string matchId = (string)(sender as Button).Tag;
            (this.DataContext as ChoseGameViewModel).joinGame(matchId);
        }
    }

}
