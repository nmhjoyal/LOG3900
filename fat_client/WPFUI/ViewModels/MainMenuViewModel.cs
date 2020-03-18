using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class MainMenuViewModel: Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private IUserData _userdata;
        private BindableCollection<Avatar> _avatars;
        public MainMenuViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _socketHandler = socketHandler;
            _userdata = userdata;
            fillAvatars();
        }

        public string username
        {
            get { return _userdata.userName; }
        }


        public void logOut()
        {
            _socketHandler.SignOut();
            _events.PublishOnUIThread(new logOutEvent());
        }
        public void viewProfile()
        {
            _events.PublishOnUIThread(new viewProfileEvent());
        }

        public void joinChatroom()
        {
            _events.PublishOnUIThread(new joinChatroomEvent());
        }

        public void viewLeaderboard()
        {
            _events.PublishOnUIThread(new leaderboardEvent());
        }

        public void freeDraw()
        {
            _events.PublishOnUIThread(new freeDrawEvent());
        }
        public void creationJeu()
        {
            _events.PublishOnUIThread(new createGameEvent());
        }

        public void joinGame()
        {
            //_events.PublishOnUIThread(new joinGameEvent());
        }
        public void fillAvatars()
        {
            _avatars = new BindableCollection<Avatar>();
            _avatars.Add(new Avatar("/Resources/apple.png", "APPLE"));
            _avatars.Add(new Avatar("/Resources/avocado.png", "AVOCADO"));
            _avatars.Add(new Avatar("/Resources/banana.png", "BANANA"));
            _avatars.Add(new Avatar("/Resources/cherry.png", "CHERRY"));
            _avatars.Add(new Avatar("/Resources/grape.png", "GRAPE"));
            _avatars.Add(new Avatar("/Resources/kiwi.png", "KIWI"));
            _avatars.Add(new Avatar("/Resources/lemon.png", "LEMON"));
            _avatars.Add(new Avatar("/Resources/orange.png", "ORANGE"));
            _avatars.Add(new Avatar("/Resources/pear.png", "PEAR"));
            _avatars.Add(new Avatar("/Resources/pineapple.png", "PINEAPPLE"));
            _avatars.Add(new Avatar("/Resources/strawberry.png", "STRAWBERRY"));
            _avatars.Add(new Avatar("/Resources/watermelon.png", "WATERMELON"));
        }

        public string avatarSource
        {
            get { return _avatars.Single(i => i.name == _userdata.avatarName).source; }
        }

    }
}
