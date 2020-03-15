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
using WPFUI.Models;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour CreationJeuManuelle2View.xaml
    /// </summary>
    public partial class CreationJeuManuelle2View : UserControl
    {
        IEventAggregator events;
        ISocketHandler socketHandler;

        public CreationJeuManuelle2View()
        {
            InitializeComponent();
            DataContext = new CreationJeuManuelle1ViewModel(events, socketHandler);
        }



        // Pour la gestion de l'affichage de position du pointeur.
        //private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e) => textBlockPosition.Text = "";
        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(surfaceDessin);
            //textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";
        }

        //code pris de: https://stackoverflow.com/questions/8881865/saving-a-wpf-canvas-as-an-image
        /*private void Button_Click(object sender, RoutedEventArgs e)
        {
            PART_Image.Source = RenderVisualService.RenderToPNGImageSource(PART_Canvas);
        }*/

        private void addClue(object sender, RoutedEventArgs e)
        {

            TextBox dynamicTextBox = new TextBox();
            Console.WriteLine("allo");

            // Grid.SetRow(dynamicTextBox, 3);
            // Grid.SetColumn(dynamicTextBox, 7);

            this.canContainer.Children.Add(dynamicTextBox);

            Console.WriteLine(this.canContainer.Children.Count);


        }




        /*private void mainMenu_Click(object sender, RoutedEventArgs e)
        {

        }*/
    }
}

