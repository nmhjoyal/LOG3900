using System;
using System.Collections.Generic;
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
		public MessageModel(string content, string senderName, DateTime timeStamp)
		{
			_content = content;
			_senderName = senderName;
			_timeStamp = timeStamp;
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

	}
}
