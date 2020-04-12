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
        string _roomSelectedToInvite;

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
            _roomSelectedToInvite = null;
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
                channelsMode.IsEnabled = false;
                ChannelText.Text = "Talk to your friends !";
            }
            else
            {
                channelsMode.IsEnabled = true;

                Binding myBinding = new Binding();
                myBinding.Source = DataContext as chatBoxViewModel;
                myBinding.Path = new PropertyPath("currentRoomId");
                myBinding.Mode = BindingMode.OneWay;
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                BindingOperations.SetBinding(ChannelText, TextBlock.TextProperty, myBinding);
            }
        }

        private void inviteButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.invitesGrid.Visibility == Visibility.Visible)
            {
                this.invitesGrid.Visibility = Visibility.Hidden;
            } else
            {
                this.addPlayersGrid.Visibility = Visibility.Hidden;
                this.newChannelGrid.Visibility = Visibility.Hidden;
                this.invitesGrid.Visibility = Visibility.Visible;
            }
        }

        private void newChannelButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            newRoomTB.Text = "";
            privateCheckBox.IsChecked = false;

            if (this.newChannelGrid.Visibility == Visibility.Visible)
            {
                this.newChannelGrid.Visibility = Visibility.Hidden;
            } else
            {
                this.addPlayersGrid.Visibility = Visibility.Hidden;
                this.invitesGrid.Visibility = Visibility.Hidden;
                this.newChannelGrid.Visibility = Visibility.Visible;
            }
        }

        private void createRoom_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string newRoomName = newRoomTB.Text;
            if ((newRoomName != null) & (newRoomName != ""))
            {
                if ((Boolean) privateCheckBox.IsChecked)
                {
                    // create private room
                    _viewModel.createRoom(newRoomName, true);
                    newRoomTB.Text = "";
                    privateCheckBox.IsChecked = false;
                } else
                {
                    // create public room
                    _viewModel.createRoom(newRoomName, false);
                    newRoomTB.Text = "";
                    privateCheckBox.IsChecked = false;
                }

            }

        }

        private void addPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            _roomSelectedToInvite = (string)(sender as Button).Tag;
            this.newChannelGrid.Visibility = Visibility.Hidden;
            this.invitesGrid.Visibility = Visibility.Hidden;
            this.addPlayersGrid.Visibility = Visibility.Visible;
        }

        private void invitePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (invitedPlayerName.Text != null & invitedPlayerName.Text != "")
            {
                _viewModel.sendInvite(_roomSelectedToInvite, invitedPlayerName.Text);
                _roomSelectedToInvite = null;
                this.addPlayersGrid.Visibility = Visibility.Hidden;
            }

        }

        private void invitePlayerButtonQuit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _roomSelectedToInvite = null;
            this.addPlayersGrid.Visibility = Visibility.Hidden;
        }

        private void joinInvitedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string _roomSelectedToJoin = (string)(sender as Button).Tag;
            _viewModel.joinInvitedRoom(_roomSelectedToJoin);
            Invitation inviteToDelete = null;
            foreach (Invitation i in _viewModel.invites)
            {
                if (i.id == _roomSelectedToJoin)
                {
                    inviteToDelete = i;
                }
            }

            if (inviteToDelete != null)
            {
                _viewModel.invites.RemoveAt(_viewModel.invites.IndexOf(inviteToDelete));
                _viewModel.invites.Refresh();
            }

        }

        private void leaveButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string _roomToLeave = (string)(sender as Button).Tag;
            _viewModel.leaveRoom(_roomToLeave);
        }

        private void deleteButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.deleteRoom();
        }
    }
}
