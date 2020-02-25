using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFUI.Commands;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class MultiChannelChatBoxViewModel: Screen
    {

        private IUserData _userdata;

        private BindableCollection<Channel> _channels;

        private BindableCollection<MessageModel> _messages;

        public BindableCollection<MessageModel> messages
        {
            get { return _messages; }
            set { _messages = value; }
        }


        public IchangeChannelCommand changeChannelCommand { get; set; }

        public BindableCollection<Channel> channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        public MultiChannelChatBoxViewModel(IUserData userdata)
        {
            _userdata = userdata;
            _channels = userdata.channels;
            _messages = _channels.Last().messages;
            changeChannelCommand = new changeChannelCommand(userdata);
        }

    }
}
