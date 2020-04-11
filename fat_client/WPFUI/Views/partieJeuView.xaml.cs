﻿using Caliburn.Micro;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFUI.EventModels;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Logique d'interaction pour partieJeuView.xaml
    /// </summary>
    public partial class partieJeuView : UserControl, IHandle<endTurnRoutineEvent>, IHandle<startTurnRoutineEvent>,
                                         IHandle<guessResponseEvent>, IHandle<endMatchEvent>
    {
        private partieJeuViewModel _viewModel;
        private Boolean isMouseDown = false;
        private Boolean firstEndTurn = true;
        private Boolean youAreDrawer = false;
        public partieJeuView()
        {
            InitializeComponent();
            firstEndTurn = true;
        }

        
        private void refocus(object sender, RoutedEventArgs e)
        {
            currentMessage.Focus();
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as partieJeuViewModel;
            (DataContext as partieJeuViewModel).events.Subscribe(this);
            messagesUI.ScrollToEnd();
        }

        public async void Handle(endTurnRoutineEvent message)
        {
            Console.WriteLine("endTurn vue.cs " + JsonConvert.SerializeObject(message));
            if (((dynamic)message.EndTurnFeedBack).nextIsYou)
            {
                youAreDrawer = true;
                roundFinishedMessage.Text = "Round " + ((dynamic)message.EndTurnFeedBack).currentRound;
                nextPlayerMessage.Text = "Next player to chose is " + ((dynamic)message.EndTurnFeedBack).drawer;
                if (!firstEndTurn)
                {
                    endTurnBox.Visibility = Visibility.Visible;
                    await Task.Delay(3000);
                    endTurnBox.Visibility = Visibility.Hidden;
                }
                selectNextDrawingBox.Visibility = Visibility.Visible;
                drawingEditingPanel.Visibility = Visibility.Visible;
                sendMessage.IsEnabled = false;
                sendGuess.IsEnabled = false;
                firstEndTurn = false;

            } else
            {
                youAreDrawer = false;
                roundFinishedMessage.Text = "Round " + ((dynamic)message.EndTurnFeedBack).currentRound;
                nextPlayerMessage.Text = "Next player to chose is " + ((dynamic)message.EndTurnFeedBack).drawer;
                if (!firstEndTurn)
                {
                    endTurnBox.Visibility = Visibility.Visible;
                }
                drawingEditingPanel.Visibility = Visibility.Collapsed;
                sendMessage.IsEnabled = true;
                sendGuess.IsEnabled = true;
                firstEndTurn = false;
            }
            (this.DataContext as partieJeuViewModel).NotifyOfPropertyChange(null);
        }

        public void Handle(startTurnRoutineEvent message)
        {
            selectNextDrawingBox.Visibility = Visibility.Hidden;
            endTurnBox.Visibility = Visibility.Hidden;
            if (!youAreDrawer)
            {
                sendMessage.IsEnabled = true;
                sendGuess.IsEnabled = true;
            }
            (this.DataContext as partieJeuViewModel).NotifyOfPropertyChange(null);
        }

        private void surfaceDessin_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                System.Windows.Point p = e.GetPosition(surfaceDessin);
                (this.DataContext as partieJeuViewModel).sendPoint((int)p.X, (int)p.Y);
            }
            //textBlockPosition.Text = Math.Round(p.X) + ", " + Math.Round(p.Y) + "px";
        }

        private void surfaceDessin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.isMouseDown)
            {
                // (this.DataContext as FenetreDessinViewModel).sendStrokeAction();
            }
            this.isMouseDown = false;
            // Console.WriteLine(this.isMouseDown);
        }

        private void surfaceDessin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
            System.Windows.Point p = e.GetPosition(surfaceDessin); ;
            (this.DataContext as partieJeuViewModel).sendStroke((int)p.X, (int)p.Y);
            // Console.WriteLine(this.isMouseDown);
        }

        private void surfaceDessin_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            (this.DataContext as partieJeuViewModel).strokeCollected(e.Stroke);
        }

        public async void Handle(guessResponseEvent message)
        {
            guessFeedbackBox.Visibility = Visibility.Visible;
            if (!message._isGoodGuess)
            {
                Storyboard sb = MarginGrid.FindResource("shakeAnimation") as Storyboard;
                sb.Begin();
            } else
            {
                sendMessage.IsEnabled = false;
                sendGuess.IsEnabled = false;
            }
            await Task.Delay(1000);
            guessFeedbackBox.Visibility = Visibility.Hidden;
        }

        public void Handle(endMatchEvent message)
        {
            endMatchBox.Visibility = Visibility.Visible;
        }
    }

}
