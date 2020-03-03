import Room from "../models/room";
import PrivateProfile from "../models/privateProfile";
import { Feedback, CreateRoomStatus, DeleteRoomStatus, JoinRoomStatus, LeaveRoomStatus, JoinRoomFeedback } from "../models/feedback";
import { roomDB } from "./Database/roomDB";
import Admin from "../models/admin";
import { Message, ClientMessage } from "../models/message";
import { profileDB } from "./Database/profileDB";
import { serverHandler } from "./serverHandler";
import ChatFilter from "./Filter/chatFilter";
import PublicProfile from "../models/publicProfile";


export default class ChatHandler {

    // await addMessage(message) ?

    public async createChatRoom(socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        let status: boolean = false;
        let log_message: CreateRoomStatus;
        try {
            const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
            if(user) {
                const publicProfile: PublicProfile = {
                    username: user.username,
                    avatar : user.avatar
                };
                await roomDB.createRoom(publicProfile, roomId);
                await profileDB.joinRoom(publicProfile.username, roomId);
                socket.join(roomId);
                const message: Message = Admin.createAdminMessage(user.username + " joined the room.", roomId);
                socket.to(roomId).emit("new_message", JSON.stringify(message));
                status = true;
                log_message = CreateRoomStatus.Create;
            } else {
                log_message = CreateRoomStatus.UserNotConnected;
            }
        } catch {
            // Room already exists
            log_message = CreateRoomStatus.AlreadyCreated
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        console.log(feedback);
        return feedback;
    }

    public async deleteChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let status: boolean = false;
        let log_message: DeleteRoomStatus;
        if(roomId == "General") {
            log_message = DeleteRoomStatus.DeleteGeneral;
        } else if(room && user) {
            let socketIds: string[] = [];
            try {
                for (let socketId in io.sockets.adapter.rooms[roomId].sockets) {
                    socketIds.push(socketId);
                }
            } catch {}
            if(socketIds.length == 0) {
                await roomDB.deleteRoom(roomId);
                await profileDB.deleteRoom(roomId);
                status = true;
                log_message = DeleteRoomStatus.Delete;
            } else if(socketIds.length == 1 && socketIds[0] == socket.id) {
                user.rooms_joined.splice(user.rooms_joined.indexOf(roomId), 1);
                socket.leave(roomId);
                await roomDB.deleteRoom(roomId);
                await profileDB.deleteRoom(roomId);
                status = true;
                log_message = DeleteRoomStatus.LeaveAndDelete;
            } else {
                log_message = DeleteRoomStatus.NotEmpty;
            }
        } else {
            log_message = DeleteRoomStatus.InvalidRoom;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        return feedback;
    }

    public async joinChatRoom(socket: SocketIO.Socket, roomId: string): Promise<JoinRoomFeedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let room_joined: Room | null = null;
        let status: boolean = false;
        let log_message: JoinRoomStatus;

        if(user && room) {
            if(user.rooms_joined.includes(room.name)) {
                log_message = JoinRoomStatus.AlreadyJoined;
            } else {
                await profileDB.joinRoom(user.username, room.name);
                user.rooms_joined.push(room.name);
                const publicProfile : PublicProfile = {
                    username : user.username,
                    avatar : user.avatar
                };
                await roomDB.mapAvatar(publicProfile, room.name);
                await this.connectToRoom(socket, user, room);
                room_joined = room;
                status = true;
                log_message = JoinRoomStatus.Join;
            }
        } else {
            log_message = JoinRoomStatus.InvalidRoom;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        const joinRoomFeedback: JoinRoomFeedback = {
            feedback: feedback,
            room_joined: room_joined
        }
        return joinRoomFeedback;
    }

    public async leaveChatRoom(socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let status: boolean = false;
        let log_message: LeaveRoomStatus;
        if(user && room) {
            if(user.rooms_joined.includes(room.name)) {
                if(roomId == "General") {
                    log_message = LeaveRoomStatus.General;
                } else {
                    await this.disconnectFromRoom(socket, user, room.name);
                    user.rooms_joined.splice(user.rooms_joined.indexOf(room.name), 1);
                    await profileDB.leaveRoom(user.username, room.name);
                    status = true;
                    log_message = LeaveRoomStatus.Leave;
                }
            } else {
                log_message = LeaveRoomStatus.NeverJoined;
            }
        } else {
            log_message = LeaveRoomStatus.InvalidRoom;
        }
        const leaveRoomFeedback: Feedback = {
            status: status,
            log_message: log_message
        }
        // console.log(leaveRoomFeedback);
        return leaveRoomFeedback;
    }

    public async sendMessage(io: SocketIO.Server, socket: SocketIO.Socket, clientMessage: ClientMessage): Promise<void> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(clientMessage.roomId);
        if(user && room) {
            // message.author.username == user.username
            if(user.rooms_joined.includes(clientMessage.roomId)) {
                const message: Message = {
                    username : user.username,
                    content : ChatFilter.filter(clientMessage.content),
                    date : Date.now(),
                    roomId : clientMessage.roomId
                }
                await roomDB.addMessage(message);

                io.in(message.roomId).emit("new_message", JSON.stringify(message));
            } else {
                console.log(user.username + " trying to send a message in " + clientMessage.roomId + " that he did not join");
            }
        } else if(user) {
            console.log(user?.username + " trying to send a message in " + clientMessage.roomId + " that does not exist");
        }
    }

    private async connectToRoom(socket: SocketIO.Socket, user: PrivateProfile, room: Room): Promise<void> {
        socket.join(room.name);
        const message: Message = Admin.createAdminMessage(user.username + " joined the room.", room.name);
        socket.to(room.name).emit("new_message", JSON.stringify(message));
        await roomDB.addMessage(message);
    }

    private async disconnectFromRoom(socket: SocketIO.Socket, user: PrivateProfile, roomId: string): Promise<void> {
        socket.leave(roomId);
        const message: Message = Admin.createAdminMessage( user.username + " left the room.", roomId);
        socket.to(roomId).emit("new_message", JSON.stringify(message));
        await roomDB.addMessage(message);
    }
}