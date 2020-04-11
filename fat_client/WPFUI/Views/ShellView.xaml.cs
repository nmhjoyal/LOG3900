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
using System.Diagnostics;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IHandle<LogInEvent>, IHandle<gameEvent>, 
                                    IHandle<goBackMainEvent>, IHandle<logOutEvent>, IHandle<joinGameEvent>,
                                    IHandle<appWarningEvent>
    {
        private string chatBoxState = "Visible";
        private Boolean chatBoxAnimating = false;
        public ShellView()
        {
            InitializeComponent();
        }

        public async void Handle(LogInEvent message)
        {
            ellipse.Visibility = Visibility.Visible;
            arrow.Visibility = Visibility.Visible;
            Storyboard sb = mainGrid.FindResource("showTopMenu") as Storyboard;
            Storyboard sb2 = mainGrid.FindResource("showChat") as Storyboard;
            sb.Begin();
            await Task.Delay(500);
            sb2.Begin();
            
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            (DataContext as ShellViewModel).events.Subscribe(this);
        }

        private void ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!chatBoxAnimating & (e.ClickCount == 1))
            {
                chatBoxAnimating = true;
                if (chatBoxState == "Visible")
                {
                    chatBoxState = "Hidden";
                    Storyboard sb2 = mainGrid.FindResource("hideChat") as Storyboard;
                    Storyboard sb3 = mainGrid.FindResource("flipArrowAnim") as Storyboard;
                    sb2.Begin();
                    sb3.Begin();
                    chatBoxAnimating = false;
                }
                else if (chatBoxState == "Hidden")
                {
                    chatBoxState = "Visible";
                    Storyboard sb2 = mainGrid.FindResource("showChat") as Storyboard;
                    Storyboard sb3 = mainGrid.FindResource("unflipArrowAnim") as Storyboard;
                    sb2.Begin();
                    sb3.Begin();
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

        public void Handle(logOutEvent message)
        {
            if (!chatBoxAnimating)
            {
                chatBoxAnimating = true;
                Storyboard sb = mainGrid.FindResource("hideTopMenu") as Storyboard;
                sb.Begin();
                if (chatBoxState == "Visible")
                {
                    Storyboard sb2 = mainGrid.FindResource("hideChat") as Storyboard;
                    sb2.Begin();
                }
                else if (chatBoxState == "Hidden")
                {
                    Storyboard sb3 = mainGrid.FindResource("unflipArrowAnim") as Storyboard;
                    sb3.Begin();
                }
                chatBoxState = "Visible";
                ellipse.Visibility = Visibility.Hidden;
                arrow.Visibility = Visibility.Hidden;
                chatBoxAnimating = false;

            }
        }

        public void Handle(joinGameEvent message)
        {
            ellipse.Visibility = Visibility.Visible;
            arrow.Visibility = Visibility.Visible;
        }

        public void Handle(appWarningEvent message)
        {
            errorMessageTB.Text = message.warningContent;
            errorBox.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            errorBox.Visibility = Visibility.Hidden;
            errorMessageTB.Text = "";
        }
    }
}
