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
    class LeaderboardViewModel : Caliburn.Micro.Screen
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


        public LeaderboardViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _soloPos = new BindableCollection<Ranking>();
            _vs1Pos = new BindableCollection<Ranking>();
            _inversedPos = new BindableCollection<Ranking>();
            _coopPos = new BindableCollection<Ranking>();
            _solo2Pos = new BindableCollection<Ranking>();

            _socketHandler = socketHandler;
            _events = events;
          
        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
    }

}
