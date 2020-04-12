using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Message
    {

        string _username;
        string _content;
        double _date;
        string _roomId;
        string _avatarSource;
        public Message(string username,
        string content,
        double date,
        string roomId)
        {
            this._username = username;
            this._content = content;
            this._roomId = roomId;
            this._date = date;
   
        }

        public string senderName
        {
            get
            {
                return _username;
            }
        }

        public string content
        {
            get
            {
                return _content;
            }
        }

        public string roomId
        {
            get
            {
                return _roomId;
            }
        }

        public string formattedTimeStamp
        {
            /*
            get
            {
                System.DateTime dtDateTime;
                if (_date != 0)
                {
                    dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(_date).ToLocalTime();
                }
                else
                {
                    dtDateTime = DateTime.Now;
                }
                return "[" + dtDateTime.ToString("HH:mm:ss") + "]";
            }
            */
            get
            {
                return "";
            }
        }

        public string avatarSource
        {
            get { return _avatarSource; }
            set { _avatarSource = value; }
        }

    }
}
