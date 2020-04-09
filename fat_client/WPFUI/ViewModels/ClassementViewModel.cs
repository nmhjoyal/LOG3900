using Caliburn.Micro;
using Newtonsoft.Json;
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
        private Ranking ranking;
        public Ranking Ranking
        {
            get { return this.ranking; }
        }
        private BindableCollection<Ranking> rankings;
        public BindableCollection<Ranking> Rankings
        {
            get { return this.rankings; }
        }

        private int mode;
        public int Mode
        {
            get { return this.mode; }
            set { this.mode = value;  this.update(mode); }
        }

        private IUserData userdata;

        public ClassementViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            // getFakeScores();

            _socketHandler = socketHandler;
            _events = events;
            this.userdata = userdata;
            this.ranking = new Ranking("", 0, 0);
            this.rankings = new BindableCollection<Ranking>();
            this.mode = 0;
            this.update(this.mode);
        }

        public void update(int index)
        {
            BindableCollection<Ranking> new_rankings = JsonConvert.DeserializeObject<BindableCollection<Ranking>>(this._socketHandler.TestGETWebRequest("/profile/rank/" + this.userdata.userName + "/" + index).ToString());
            this.rankings.Clear();
            this.ranking.set(new_rankings[new_rankings.Count - 1]);
            this.rankings.AddRange(new_rankings.Take(new_rankings.Count - 1));
            NotifyOfPropertyChange(null);
        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
    }

}