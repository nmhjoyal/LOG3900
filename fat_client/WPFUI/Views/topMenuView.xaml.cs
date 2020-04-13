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
using WPFUI.EventModels;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for topMenuView.xaml
    /// </summary>
    public partial class topMenuView : UserControl, IHandle<buttonsTopMenuEvent>
    {
        topMenuViewModel vm;
        public topMenuView()
        {
            InitializeComponent();
        }

        public void Handle(buttonsTopMenuEvent message)
        {
            if (message.hide == false)
            {
                goToScores.Visibility = Visibility.Collapsed;
                goToProfileEdit.Visibility = Visibility.Collapsed;
            } else
            {
                goToScores.Visibility = Visibility.Visible;
                goToProfileEdit.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as topMenuViewModel;
            vm.events().Subscribe(this);
        }
    }
}
