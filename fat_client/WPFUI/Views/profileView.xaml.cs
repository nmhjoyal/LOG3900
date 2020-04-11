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

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour profileView.xaml
    /// </summary>
    public partial class profileView : UserControl
    {
        public profileView()
        {
            InitializeComponent();
        }

        private void saveProfileChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        private void matchHistory(object sender, RoutedEventArgs e)
        {
            this.border.Visibility = Visibility.Visible;
            this.selectNextDrawingBox.Visibility = Visibility.Visible;
        }

        private void close(object sender, RoutedEventArgs e)
        {
            this.border.Visibility = Visibility.Hidden;
            this.selectNextDrawingBox.Visibility = Visibility.Hidden;
        }
    }
}
