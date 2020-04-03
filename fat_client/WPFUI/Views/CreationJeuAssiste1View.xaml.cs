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
    /// Logique d'interaction pour CreationJeuAssiste1View.xaml
    /// </summary>
    public partial class CreationJeuAssiste1View : UserControl
    {
        public CreationJeuAssiste1View()
        {
            InitializeComponent();
        }



        // Pour la gestion de l'affichage de position du pointeur.
        //private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e) => textBlockPosition.Text = "";


        private void addClue(object sender, RoutedEventArgs e)
        {

            TextBox dynamicTextBox = new TextBox();
            Console.WriteLine("allo");

            // Grid.SetRow(dynamicTextBox, 3);
            // Grid.SetColumn(dynamicTextBox, 7);

            this.canContainer.Children.Add(dynamicTextBox);
            dynamicTextBox.Name = "indice" + this.canContainer.Children.Count;
            Console.WriteLine(dynamicTextBox.Name);

            Console.WriteLine(this.canContainer.Children.Count);


        }

        private void createGame(object sender, RoutedEventArgs e)
        {
            List<string> clues = new List<string>();
            for (int i = 0; i < this.canContainer.Children.Count; i++)
            {
                clues.Add((this.canContainer.Children[i] as TextBox).Text);

            }
            int option = -1;
            if (this.Options.Children.Count > 0)
            {
                option = (this.Options.Children[0] as ComboBox).SelectedIndex;
            }
            Console.WriteLine(JsonConvert.SerializeObject(clues));
            (this.DataContext as CreationJeuAssiste1ViewModel).createGame(this.Word.Text, clues, this.Level.SelectedIndex, this.Mode.SelectedIndex, option, this.fileName.Text, (int)this.imageTransformee.ActualWidth, (int)this.imageTransformee.ActualHeight);
        }

        private void preview(object sender, RoutedEventArgs e)
        {
            int option = -1;
            if (this.Options.Children.Count > 0)
            {
                option = (this.Options.Children[0] as ComboBox).SelectedIndex;
            }
            (this.DataContext as CreationJeuAssiste1ViewModel).preview(this.fileName.Text, this.Mode.SelectedIndex, option, (int)this.imageTransformee.ActualWidth, (int)this.imageTransformee.ActualHeight);
        }

        private void elementSelectionne(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(this.Mode.SelectedIndex);
            ComboBox comboBox = new ComboBox();
            TextBlock text = new TextBlock();
            text.Text = "Options:";
            text.TextAlignment = TextAlignment.Center;

            this.Options.Children.Clear();
            this.optionBlock.Children.Clear();
            if (this.Mode.SelectedIndex == 2)
            {
                this.optionBlock.Children.Add(text);
                comboBox.Items.Add("left to right");
                comboBox.Items.Add("right to left");
                comboBox.Items.Add("top to bottom");
                comboBox.Items.Add("bottom to top");
                comboBox.SelectedIndex = 0;
                this.Options.Children.Add(comboBox);
            }

            else if (this.Mode.SelectedIndex == 3)
            {
                this.optionBlock.Children.Add(text);
                comboBox.Items.Add("From inside out");
                comboBox.Items.Add("From outside in");
                comboBox.SelectedIndex = 0;
                this.Options.Children.Add(comboBox);
            }

        }

        private void imageTransformee_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            (this.DataContext as CreationJeuAssiste1ViewModel).preventDrawing(e.Stroke);
        }
    }
}
