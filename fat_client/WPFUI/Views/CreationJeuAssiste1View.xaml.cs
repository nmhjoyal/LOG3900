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
        IEventAggregator events;
        ISocketHandler socketHandler;

        public CreationJeuAssiste1View()
        {
            InitializeComponent();
            DataContext = new CreationJeuAssiste1ViewModel(events, socketHandler);
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

        private void createManGame2(object sender, RoutedEventArgs e)
        {
            List<string> clues = new List<string>();
            for (int i = 2; i < this.canContainer.Children.Count; i++)
            {
                clues.Add((this.canContainer.Children[i] as TextBox).Text);

            }
            Console.WriteLine(JsonConvert.SerializeObject(clues));


        }



        private void preview(object sender, RoutedEventArgs e)
        {

        }
    }
}
