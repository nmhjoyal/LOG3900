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

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for NewUserView.xaml
    /// </summary>
    public partial class NewUserView : UserControl
    {
        NewUserViewModel vm; 
        public NewUserView()
        {
            InitializeComponent();
        }

        private void rightArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.rightArrowAvatarChange();
        }

        private void leftArrow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.leftArrowAvatarChange();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            vm = (DataContext as NewUserViewModel);
        }

        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            vm.confirmedPassword = confirmedPassword.Password;
            vm.password = password.Password;
            vm.createUser();

        }
    }
}
