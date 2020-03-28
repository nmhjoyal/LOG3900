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
    /// Logique d'interaction pour createMatchView.xaml
    /// </summary>
    public partial class createMatchView : UserControl
    {
        public createMatchView()
        {
            InitializeComponent();
        }

        private void createMatch(object sender, RoutedEventArgs e)
        {
            MatchMode matchMode = (MatchMode)this.modecomboBox.SelectedIndex;
            int nbRounds = int.Parse(this.roundcomboBox.Text);
            int timeLimit = int.Parse(this.timecomboBox.Text.Substring(0, this.timecomboBox.Text.Length - 8));
            (this.DataContext as createMatchViewModel).createMatch(matchMode, nbRounds, timeLimit);
        }
    }
}
