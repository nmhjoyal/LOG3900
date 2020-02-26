using Caliburn.Micro;
using Microsoft.Win32;
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
    /// Logique d'interaction pour FenetreDessinView.xaml
    /// </summary>
    public partial class FenetreDessinView : UserControl
    {
        IEventAggregator events;
        ISocketHandler socketHandler;
        public FenetreDessinView()
        {
            InitializeComponent();
            DataContext = new FenetreDessinViewModel(events,socketHandler);
        }



        // Pour la gestion de l'affichage de position du pointeur.
        //private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e) => textBlockPosition.Text = "";
        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(surfaceDessin);
            //textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";
        }

        //code pris de: https://stackoverflow.com/questions/8881865/saving-a-wpf-canvas-as-an-image
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PART_Image.Source = RenderVisualService.RenderToPNGImageSource(PART_Canvas);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.FileName = txtEditor.Text;
            if (saveFileDialog.ShowDialog() == true) {
                RenderVisualService.RenderToPNGFile(PART_Canvas, saveFileDialog.FileName);
            }
                
        }

        private void mainMenu_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
