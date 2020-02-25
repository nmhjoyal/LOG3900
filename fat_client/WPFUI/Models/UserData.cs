using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class UserData : IUserData
    {
        private string _userName;
        private string _password;
        private string _ipAdress;
        private string _currentMessage;
        private BindableCollection<MessageModel> _messages;

        private BindableCollection<Channel> _channels;

        public BindableCollection<Channel> channels
        {
            get { return _channels; }
            set { _channels = value; }
        }


        public BindableCollection<MessageModel> messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value; }
        }


        public string userName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string ipAdress
        {
            get { return _ipAdress; }
            set { _ipAdress = value; }
        }

        public string password
        {
            get { return _password; }
            set { _password = value; }
        }


        public UserData(string userName, string ipAdress, string password)
        {
            _userName = userName;
            _ipAdress = ipAdress;
            _password = password;
            _messages = new BindableCollection<MessageModel>();
            _channels = new BindableCollection<Channel>();
            addFakeChannels();
            _messages = _channels.Last().messages;
        }

        public void addFakeChannels()
        {
            _channels.Add(new Channel(1, "channel1"));
            _channels.Add(new Channel(4, "channel3"));
            _channels.Add(new Channel(9, "channel5"));
        }
        public void clearData()
        {
            _currentMessage = "";
            _userName = "";
            _ipAdress = "";
            _password = "";
            _messages = new BindableCollection<MessageModel>();

        }

        public void changeChannel(int channelID)
        {
            messages = _channels.Single(i => i.channelID == channelID).messages;
        }

    }
}
