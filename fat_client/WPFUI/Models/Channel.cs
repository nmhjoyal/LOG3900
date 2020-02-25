﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Channel
    {
        private BindableCollection<MessageModel> _messages;

        private string _channelName;

        public string channelName
        {
            get { return _channelName; }
            set { _channelName = value; }
        }

        public BindableCollection<MessageModel> messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public Channel()
        {
            _channelName = "Default Channel Name";
            _messages = new BindableCollection<MessageModel>();
            addFakeMessages();
        }

        public void addFakeMessages()
        {
            _messages.Add(new MessageModel("hello", "bob", 0));
            _messages.Add(new MessageModel("whatsup bob", "denis", 0));
            _messages.Add(new MessageModel("jsuis a la maisonnee", "bob", 0));
            _messages.Add(new MessageModel("ahh okay, tres nice", "denis", 0));
        }

        public string lastMessageContent
        {
            get {
                return _messages.Last().content;
                }
        }

        public string formattedTimeStamp
        {
            get{
                return _messages.Last().formattedTimeStamp;
               }
        }

    }
}
