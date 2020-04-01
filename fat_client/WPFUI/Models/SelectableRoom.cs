﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class SelectableRoom
    {
        private Room _room;
        private string _backgroundColor;
        private string _menuVisibility;

        public SelectableRoom(Room room)
        {
            _room = room;
            _backgroundColor = "#FF66CC2F";
            _menuVisibility = "Collapsed";
        }

        public Room room
        {
            get { return _room; }
        }

        public string menuVisibility
        {
            get { return _menuVisibility; }
            set { _menuVisibility = value; }
        }

        public string id
        {
            get { return _room.id; }
        }

        public string backgroundColor
        {
            get { return _backgroundColor; }
        }

        public void changeColor(string color)
        {
            _backgroundColor = color;
        }

        public void resetColor()
        {
            _backgroundColor = "#FF66CC2F";
        }
    }
}