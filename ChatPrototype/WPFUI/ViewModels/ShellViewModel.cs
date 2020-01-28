using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class ShellViewModel: Conductor<object>
    {
		private string _chatName;
		private string _ipAdress;
		private BindableCollection<MessageModel> _messages = new BindableCollection<MessageModel>();

		public ShellViewModel()
		{

		}

		private string _connectMessage;

		public string connectMessage
		{
			get { return $"Tentative de connection de { chatName } avec l'IP { ipAdress }"; }
		}

		public BindableCollection<MessageModel> messages
		{
			get { return _messages; }
			set { _messages = value; }
		}

		public string ipAdress
		{
			get { return _ipAdress; }
			set { _ipAdress = value;
				NotifyOfPropertyChange(() => ipAdress);
				NotifyOfPropertyChange(() => connectMessage);
			}
		}


		public string chatName
		{
			get { return _chatName; }
			set { _chatName = value;
				  NotifyOfPropertyChange(() => chatName);
				  NotifyOfPropertyChange(() => connectMessage);}
		}

		public void loadChat()
		{

			ActivateItem(new chatBoxViewModel());
		}
	}

}
