using Caliburn.Micro;
using Newtonsoft.Json;
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
        public CreationJeuManuelle2View()
        {
            InitializeComponent();
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
            this.canContainer.Children.Add(new TextBox());
        }

        private void createGame(object sender, RoutedEventArgs e)
        {
            List<string> clues = new List<string>();
            for(int i = 0; i < this.canContainer.Children.Count; i++)
            {
                clues.Add((this.canContainer.Children[i] as TextBox).Text);
            }
            int option = -1;
            if(this.Options.Children.Count > 0)
            {
                option = (this.Options.Children[0] as ComboBox).SelectedIndex;
            }
            (this.DataContext as CreationJeuManuelle2ViewModel).createGame(this.Word.Text, clues, this.Level.SelectedIndex, this.Mode.SelectedIndex, option);
            
        }

        private void preview(object sender, RoutedEventArgs e)
        {
            int option = -1;
            if (this.Options.Children.Count > 0)
            {
                option = (this.Options.Children[0] as ComboBox).SelectedIndex;
            }
            (this.DataContext as CreationJeuManuelle2ViewModel).preview(this.Mode.SelectedIndex, option);
        }
        private void elementSelectionne(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = new ComboBox();
            TextBlock text = new TextBlock();
            text.Text = "Options:";
            text.TextAlignment = TextAlignment.Center;

            this.Options.Children.Clear();
            this.optionBlock.Children.Clear();
            if (this.Mode.SelectedIndex==2)
            {
                this.optionBlock.Children.Add(text);
                comboBox.Items.Add("De gauche à droite");
                comboBox.Items.Add("De droite à gauche");
                comboBox.Items.Add("De haut en bas");
                comboBox.Items.Add("De bas en haut");
                comboBox.SelectedIndex = 0;
                this.Options.Children.Add(comboBox);
            }

            else if (this.Mode.SelectedIndex == 3)
            {
                this.optionBlock.Children.Add(text);
                comboBox.Items.Add("De l'intérieur vers l'extérieur");
                comboBox.Items.Add("Del'extérieur vers l'intérieur");
                comboBox.SelectedIndex = 0;
                this.Options.Children.Add(comboBox);
            }

        }
    }
}

