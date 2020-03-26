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
using System.Windows.Shapes;
using WPFUI.ViewModels;
using Caliburn;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for chatBoxWindowView.xaml
    /// </summary>
    public partial class chatBoxWindowView : Window
    {
        public chatBoxWindowView()
        {
            InitializeComponent();
        }
        private void refocus(object sender, RoutedEventArgs e)
        {
            currentMessage.Focus();
        }

        private void currentMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //  _viewModel.sendMessage(currentMessage.Text);
            }

        }
    }
}
