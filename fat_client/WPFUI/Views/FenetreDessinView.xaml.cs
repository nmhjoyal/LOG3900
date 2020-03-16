﻿using Caliburn.Micro;
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
        private Boolean isMouseDown = false;
        public FenetreDessinView()
        {
            InitializeComponent();
            // this.surfaceDessin.InkPresenter
            // DataContext = new FenetreDessinViewModel(events, socketHandler, surfaceDessin);
        }



        // Pour la gestion de l'affichage de position du pointeur.
        //private void surfaceDessin_MouseLeave(object sender, MouseEventArgs e) => textBlockPosition.Text = "";
        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            if(this.isMouseDown)
            {
                Console.WriteLine("mouse move");
                System.Windows.Point p = e.GetPosition(surfaceDessin);
                (this.DataContext as FenetreDessinViewModel).sendPointAction((int)p.X, (int)p.Y);
            }
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
            if (saveFileDialog.ShowDialog() == true)
            {
                RenderVisualService.RenderToPNGFile(PART_Canvas, saveFileDialog.FileName);
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("should work");
            (this.DataContext as FenetreDessinViewModel).sendDrawing();
        }

        private void mainMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void surfaceDessin_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Console.WriteLine("collected");
        }
        private void surfaceDessin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(this.isMouseDown) {
                // (this.DataContext as FenetreDessinViewModel).sendStrokeAction();
            }
            this.isMouseDown = false;
            // Console.WriteLine(this.isMouseDown);
        }

        private void surfaceDessin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
            System.Windows.Point p = e.GetPosition(surfaceDessin);
            Console.WriteLine("mouse down");
            (this.DataContext as FenetreDessinViewModel).sendStrokeAction((int)p.X, (int)p.Y);
            // Console.WriteLine(this.isMouseDown);
        }

        private void surfaceDessin_StrokesReplaced(object sender, InkCanvasStrokesReplacedEventArgs e)
        {

        }
    }
}
