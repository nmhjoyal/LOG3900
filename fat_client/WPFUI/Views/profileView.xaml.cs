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
    /// Logique d'interaction pour profileView.xaml
    /// </summary>
    public partial class profileView : UserControl, IHandle<avatarUpdated>
    {
        profileViewModel vm;
        public profileView()
        {
            InitializeComponent();
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
            this.borderconnection.Visibility = Visibility.Hidden;
            this.connectdisconnect.Visibility = Visibility.Hidden;
        }
        private void openconnectdisconnect(object sender, RoutedEventArgs e)
        {
            
            this.borderconnection.Visibility = Visibility.Visible;
            this.connectdisconnect.Visibility = Visibility.Visible;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            vm = (DataContext as profileViewModel);
            vm.events.Subscribe(this);
            passwordTB.Password = vm.initialPP.password;
            password2TB.Password = vm.initialPP.password;
        }

        private void saveChangesButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.saveChanges(passwordTB.Password, password2TB.Password, newFirstNameBox.Text, newLastNameBox.Text);
        }

        private void rightArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.rightArrowAvatarChange();
        }

        private void leftArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.leftArrowAvatarChange();
        }

        private void editProfileButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(editProfileGrid.Visibility == Visibility.Visible)
            {
                editProfileGrid.Visibility = Visibility.Hidden;
            } else
            {
                editProfileGrid.Visibility = Visibility.Visible;
            }
        }

        public void Handle(avatarUpdated message)
        {
            passwordTB.Password = vm.initialPP.password;
            password2TB.Password = vm.initialPP.password;
        }

        private void deleteProfileButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.deleteAccount();
        }
    }
}
