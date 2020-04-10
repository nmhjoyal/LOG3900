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
using WPFUI.ViewModels;
using WPFUI.EventModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour WaitingRoomView.xaml
    /// </summary>
    public partial class WaitingRoomView : UserControl
    {
       

        WaitingRoomViewModel _viewModel;
        public WaitingRoomView()
        {
            InitializeComponent();
        }
        private void refocus(object sender, RoutedEventArgs e)
        {
            currentMessage.Focus();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as WaitingRoomViewModel;
            _viewModel.events.PublishOnUIThread(new changeChatOptionsEvent(false));
        }

        private void currentMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //  _viewModel.sendMessage(currentMessage.Text);
            }

        }

        public void Button_Click(object sender, MouseButtonEventArgs e)
        {
            if ((e.ClickCount == 1))
            {
                _viewModel.start();
            }
        }
    }
}
