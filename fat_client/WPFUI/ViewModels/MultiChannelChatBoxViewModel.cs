using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class MultiChannelChatBoxViewModel: Screen
    {
        private BindableCollection<Channel> _channels;

        public BindableCollection<Channel> channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        public MultiChannelChatBoxViewModel()
        {
            _channels = new BindableCollection<Channel>();
            getFakeChannels();
        }
        public void getFakeChannels()
        {
            channels.Add(new Channel());
            channels.Add(new Channel());
            channels.Add(new Channel());
        }
    }
}
