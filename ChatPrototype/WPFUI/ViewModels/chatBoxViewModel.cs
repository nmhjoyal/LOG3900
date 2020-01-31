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
    public class chatBoxViewModel: Screen
    {
        private IEventAggregator _events;
        private List<MessageModel> _messages;
        private IUserData _userData;


        public chatBoxViewModel(IUserData userdata, IEventAggregator events)
        {
            _events = events;
            _messages = new List<MessageModel>();
            _userData = userdata;
        }

        public string welcomeMessage
        {
            get
            {
                return $"Username: {_userData.userName} Server IP Adress: {_userData.ipAdress} ";
            }
        }
        public void disconnect()
        {
            _userData.ipAdress = "";
            _userData.userName = "";
            _events.PublishOnUIThread(new DisconnectEvent());
        }
    }

}
