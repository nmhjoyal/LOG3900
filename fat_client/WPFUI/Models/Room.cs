﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Room
    {
        public string id;
        public Message[] messages;
        public Dictionary<string, string> avatars;

        public Room(string id,
        Message[] messages,
        Dictionary<string, string> avatars)
        {
            this.id = id;
            this.messages = messages;
            this.avatars = avatars;
        }
        public string roomName
        {
            get { return id; }
            set { id = value; }
        }
    }
}
