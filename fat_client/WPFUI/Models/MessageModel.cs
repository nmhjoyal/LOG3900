using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class MessageModel
    {
		private string _content;
		private string _senderName;
		private DateTime _timeStamp;
		public MessageModel(string content, string senderName, double unixTimeStamp = 0 )
		{
			_content = content;
			_senderName = senderName;
			if (unixTimeStamp !=0)
			{
				System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
				dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
				_timeStamp = dtDateTime;
			} else
			{
				_timeStamp = DateTime.Now;
			}
			
		}

		public string content
		{
			get { return _content; }
			set { _content = value; }
		}

		public string senderName
		{
			get { return _senderName; }
			set { _senderName = value; }
		}

		public DateTime timeStamp
		{
			get { return _timeStamp; }
			set { _timeStamp = value; }
		}

		public string formattedTimeStamp
		{
			get
			{
				return "["+timeStamp.ToString("HH:mm:ss") +"]";
			}
		}

	}
}
