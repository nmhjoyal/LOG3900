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
using System.Windows.Threading;
using WPFUI.EventModels;
using WPFUI.Models;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour chatBoxView.xaml
    /// </summary>
    public partial class chatBoxView : UserControl, IHandle<refreshMessagesEvent>, IHandle<scrollDownEvent>,
                                       IHandle<changeChatOptionsEvent>

    {
        chatBoxViewModel _viewModel;
        IEventAggregator _events;
        IUserData _userdata;

        public chatBoxView()
        {
            InitializeComponent();
        }

        private void refocus(object sender, RoutedEventArgs e)
        {
            currentMessage.Focus();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as chatBoxViewModel;
            _events = (DataContext as chatBoxViewModel).events;
            _events.Subscribe(this);
            _userdata = (DataContext as chatBoxViewModel).userdata;
            messagesUI.ScrollToBottom();
        }


        private void channelsMode_Click(object sender, RoutedEventArgs e)
        {
            if (channelsGrid.Visibility == Visibility.Hidden)
            {
                channelsGrid.Visibility = Visibility.Visible;
                //channelsGrid.Focus();
                messagesUI.Opacity = 0.5;
            }
            else
            {
                channelsGrid.Visibility = Visibility.Hidden;
                //this.Focus();
                messagesUI.Opacity = 1;
            }

        }

        private void joinButton_click(object sender, RoutedEventArgs e)
        {
            (DataContext as chatBoxViewModel).joinRoom();
        }

        public void Handle(refreshMessagesEvent message)
        {
            messagesUI.ScrollToEnd();
        }

        public void Handle(scrollDownEvent message)
        {
            messagesUI.ScrollToEnd();
        }

        public void Handle(changeChatOptionsEvent message)
        {
            if (!message.visible)
            {
                Console.WriteLine("message.visible = false");
                channelsMode.IsEnabled = false;
                ChannelText.Text = "Talk to your friends !";
            }
            else
            {
                Console.WriteLine("message.visible = true");
                channelsMode.IsEnabled = true;
                ChannelText.Text = "{Binding Path = currentRoomId, Mode = Oneway}";
            }
        }
    }
}
