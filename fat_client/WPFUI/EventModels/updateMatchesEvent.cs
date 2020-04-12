using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.EventModels
{
    class updateMatchesEvent
    {
        public BindableCollection<Match> matches;

        public updateMatchesEvent(BindableCollection<Match> matches)
        {
            this.matches = new BindableCollection<Match>(matches);
        }
    }
}
