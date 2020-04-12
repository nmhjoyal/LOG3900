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
        long _date;
        string _roomId;
        string _avatarSource;
        public Message(string username,
        string content,
        long date,
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
            get
            {
                return "[" + UnixTimeToDateTime(_date).ToString("HH:mm:ss") + "]";
            }
        }

        public string avatarSource
        {
            get { return _avatarSource; }
            set { _avatarSource = value; }
        }


        //methode prise de: https://ourcodeworld.com/articles/read/865/how-to-convert-an-unixtime-to-datetime-class-and-viceversa-in-c-sharp
        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }

    }
}
