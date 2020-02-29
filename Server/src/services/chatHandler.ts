import Room from "../models/room";
import PrivateProfile from "../models/privateProfile";
import { Feedback, CreateRoomStatus, JoinRoomStatus, LeaveRoomStatus } from "../models/feedback";
import { roomDB } from "./Database/roomDB";
import Admin from "../models/admin";
import { Message, ClientMessage } from "../models/message";
import { profileDB } from "./Database/profileDB";
import { serverHandler } from "./serverHandler";
import ChatFilter from "./Filter/chatFilter";


export default class ChatHandler {

    public async createChatRoom(roomId: string): Promise<Feedback> {
        let status: boolean = false;
        let log_message: CreateRoomStatus;
        try {
            await roomDB.createRoom(roomId);
            status = true;
            log_message = CreateRoomStatus.Create;
        } catch {
            // Room already exists
            log_message = CreateRoomStatus.AlreadyCreated
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        return feedback;
    }

    public async joinChatRoom(socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let status: boolean = false;
        let log_message: JoinRoomStatus;

        if(user && room) {
            if(user.rooms_joined.includes(room.name)) {
                log_message = JoinRoomStatus.AlreadyJoined;
            } else {
                await profileDB.joinRoom(user.username, room.name);
                this.connectToRoom(socket, user, room);
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
        return feedback;
    }

    public async leaveChatRoom(socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let status: boolean = false;
        let log_message: LeaveRoomStatus;

        if(user && room) {
            if(user.rooms_joined.includes(room.name)) {
                this.disconnectFromRoom(socket, user, room.name);
                await profileDB.leaveRoom(user.username, room.name);
                status = true;
                log_message = LeaveRoomStatus.Leave;
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

    private connectToRoom(socket: SocketIO.Socket, user: PrivateProfile, room: Room): void {
        socket.join(room.name);
        const message: Message = Admin.createAdminMessage(user.username + " joined the room.", room.name);
        socket.to(room.name).emit("new_message", JSON.stringify(message));
        socket.emit("load_history", JSON.stringify({
            name: room.name,
            messages: room.messages
        }));
    }

    private disconnectFromRoom(socket: SocketIO.Socket, user: PrivateProfile, roomId: string) {
        socket.leave(roomId);
        const message: Message = Admin.createAdminMessage( user.username + " left the room.", roomId);
        socket.to(roomId).emit("new_message", JSON.stringify(message));
    }
}