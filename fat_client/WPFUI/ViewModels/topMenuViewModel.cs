using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class topMenuViewModel: Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private IUserData _userData;

        public topMenuViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _userData = userdata;
            _events = events;
            _socketHandler = socketHandler;
            _events.Subscribe(this);
        }

        public void goToScores()
        {
            _events.PublishOnUIThread(new LeaderboardEvent());
        }

        public void goToProfileEdit()
        {
            _events.PublishOnUIThread(new viewProfileEvent());
        }

        public void disconnect()
        {
            _socketHandler.SignOut();
        }

        public void goToMenu()
        {
            if (_userData.matchId != null)
            {
                leaveMatchRoutine();
            }
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void leaveMatchRoutine()
        {
            _socketHandler.socket.Emit("leave_chat_room", _userData.matchId);
            _userData.matchId = null;
            _userData.currentGameRoom = null;
            Room general = _userData.selectableJoinedRooms[0].room;
            _userData.messages = new BindableCollection<Models.Message>(general.messages);
            _userData.currentRoomId = general.roomName;
            _events.PublishOnUIThread(new refreshMessagesEvent(_userData.messages, _userData.currentRoomId));
        }


    }
}
