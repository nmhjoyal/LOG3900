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

		public string content
		{
			get { return _content; }
			set { _content = value; }
		}

		private string _senderName;

		public string senderName
		{
			get { return _senderName; }
			set { _senderName = value; }
		}

		private DateTime _timeStamp;

		public DateTime timeStamp
		{
			get { return _timeStamp; }
			set { _timeStamp = value; }
		}

	}
}
