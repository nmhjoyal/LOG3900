using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class ClassementViewModel: Caliburn.Micro.Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;

        private BindableCollection<Ranking> _soloPos;

        public BindableCollection<Ranking> soloPos
        {
            get { return _soloPos; }
            set { _soloPos = value; }
        }

        private BindableCollection<Ranking> _vs1Pos;

        public BindableCollection<Ranking> vs1Pos
        {
            get { return _vs1Pos; }
            set { _vs1Pos = value; }
        }

        private BindableCollection<Ranking> _inversedPos;

        public BindableCollection<Ranking> inversedPos
        {
            get { return _inversedPos; }
            set { _inversedPos = value; }
        }

        private BindableCollection<Ranking> _coopPos;

        public BindableCollection<Ranking> coopPos
        {
            get { return _coopPos; }
            set { _coopPos = value; }
        }

        private BindableCollection<Ranking> _solo2Pos;

        public BindableCollection<Ranking> solo2pos
        {
            get { return _solo2Pos; }
            set { _solo2Pos = value; }
        }


        public ClassementViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _soloPos = new BindableCollection<Ranking>();
            _vs1Pos = new BindableCollection<Ranking>();
            _inversedPos = new BindableCollection<Ranking>();
            _coopPos = new BindableCollection<Ranking>();
            _solo2Pos = new BindableCollection<Ranking>();

            getFakeScores();

            _socketHandler = socketHandler;
            _events = events;

        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void getFakeScores()
        {
            soloPos.Add(new Ranking("karima", 1));
            soloPos.Add(new Ranking("nicole", 2));
            soloPos.Add(new Ranking("hubert", 3));
            soloPos.Add(new Ranking("olivierG", 4));
            soloPos.Add(new Ranking("sebG", 5));
            soloPos.Add(new Ranking("log3000", 6));
            soloPos.Add(new Ranking("log2410", 7));
            soloPos.Add(new Ranking("log2610", 8));
            soloPos.Add(new Ranking("tanguy", 9));
            soloPos.Add(new Ranking("covid-19", 10));
            soloPos.Add(new Ranking("h1n1", 11));
            soloPos.Add(new Ranking("vacheFolle", 12));
        }
    }

}