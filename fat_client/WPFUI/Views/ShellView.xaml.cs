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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFUI.EventModels;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IHandle<LogInEvent>, IHandle<gameEvent>, IHandle<goBackMainEvent>
    {
        private string chatBoxState = "Visible";
        private Boolean chatBoxAnimating = false;
        public ShellView()
        {
            InitializeComponent();
        }

        public void Handle(LogInEvent message)
        {
            Storyboard sb = mainGrid.FindResource("showTopMenu") as Storyboard;
            sb.Begin();
            Storyboard sb2 = mainGrid.FindResource("showChat") as Storyboard;
            sb2.Begin();
            ellipse.Visibility = Visibility.Visible;
            arrow.Visibility = Visibility.Visible;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            (DataContext as ShellViewModel).events.Subscribe(this);
        }

        private void ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!chatBoxAnimating & !(e.ClickCount >= 2))
            {
                chatBoxAnimating = true;
                if (chatBoxState == "Visible")
                {
                    Storyboard sb2 = mainGrid.FindResource("hideChat") as Storyboard;
                    Storyboard sb3 = mainGrid.FindResource("flipArrowAnim") as Storyboard;
                    sb2.Begin();
                    sb3.Begin();
                    chatBoxState = "Hidden";
                    chatBoxAnimating = false;
                }
                else if (chatBoxState == "Hidden")
                {
                    Storyboard sb2 = mainGrid.FindResource("showChat") as Storyboard;
                    Storyboard sb3 = mainGrid.FindResource("unflipArrowAnim") as Storyboard;
                    sb2.Begin();
                    sb3.Begin();
                    chatBoxState = "Visible";
                    chatBoxAnimating = false;
                }

            }

        }


        public void Handle(gameEvent message)
        {
            ellipse.Visibility = Visibility.Hidden;
            arrow.Visibility = Visibility.Hidden;

            if (chatBoxState == "Visible")
            {
                Storyboard sb2 = mainGrid.FindResource("hideChat") as Storyboard;
                Storyboard sb3 = mainGrid.FindResource("flipArrowAnim") as Storyboard;
                sb2.Begin();
                sb3.Begin();
                chatBoxState = "Hidden";
                chatBoxAnimating = false;
            }

        }

        public void Handle(goBackMainEvent message)
        {
            ellipse.Visibility = Visibility.Visible;
            arrow.Visibility = Visibility.Visible;
        }
    }
}
