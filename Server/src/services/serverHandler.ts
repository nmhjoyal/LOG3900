import SignIn from "../models/signIn";
import Message from "../models/message"
import ClientMessage from "../models/message"
import PrivateProfile from "../models/privateProfile";
import Room from "../models/room";
import Feedback from "../models/feedback";
import { SignInFeedback } from "../models/feedback";
import { SignInStatus } from "../models/feedback";
import { SignOutStatus } from "../models/feedback";
import { CreateRoomStatus } from "../models/feedback";
import { JoinRoomStatus } from "../models/feedback";
import { LeaveRoomStatus } from "../models/feedback";
import Admin from "../models/admin";
import { profileDB } from "../services/Database/profileDB";
import { roomDB } from "../services/Database/roomDB";
// import ChatFilter from "./chatFilter";

export default class ServerHandler {
    private users: Map<string, PrivateProfile>;

    public constructor() {
        this.users = new Map();
    }

    public async signIn(socket: SocketIO.Socket, signIn: SignIn): Promise<SignInFeedback> {
        const user: PrivateProfile | null = await profileDB.getPrivateProfile(signIn.username);
        let signed_in: boolean = false;
        let log_message: SignInStatus;
        let rooms_joined: string[] = [];
        if(user) {
            if(signIn.password == user.password) {
                if(this.isConnected(signIn.username)) {
                    log_message = SignInStatus.AlreadyConnected
                } else {
                    this.users.set(socket.id, user);
                    signed_in = true;
                    log_message = SignInStatus.SignIn;
                    rooms_joined = user.rooms_joined;
                    user.rooms_joined.forEach(async (room_joined: string) => {
                        socket.join(room_joined);
                        const message: Message = Admin.createAdminMessage(user.username + " is connected.", room_joined);
                        socket.to(room_joined).emit("new_message", JSON.stringify(message));
                        const room: Room | null = await roomDB.getRoom(room_joined);
                        if(room) {
                            socket.emit("load_history", JSON.stringify({
                                name: room.name,
                                messages: room.messages
                            }));
                        } else {
                            console.log("This room does not exist : " + room_joined);
                        }
                    });
                }
            } else {
                log_message = SignInStatus.InvalidPassword;
            }
        } else {
            log_message = SignInStatus.InvalidUsername;
        }
        const feedback: Feedback = {
            status: signed_in,
            log_message: log_message
        }
        const signInFeedback: SignInFeedback = {
            feedback: feedback,
            rooms_joined: rooms_joined
        }
        return signInFeedback;
    }

    public async signOut(socket: SocketIO.Socket): Promise<Feedback> {
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        let status: boolean = false;
        let log_message: SignOutStatus = SignOutStatus.Error;
        if(user) {
            user.rooms_joined.forEach((room_joined: string) => {
                const message: Message = Admin.createAdminMessage(user.username + " is disconnected.", room_joined);
                socket.to(room_joined).emit("new_message", JSON.stringify(message));
                socket.leave(room_joined);
            });
            this.users.delete(socket.id);
            status = true;
            log_message = SignOutStatus.SignedOut;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        return feedback;
    }

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
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let status: boolean = false;
        let log_message: JoinRoomStatus;

        if(user && room) {
            if(user.rooms_joined.includes(roomId)) {
                log_message = JoinRoomStatus.AlreadyJoined;
            } else {
                await profileDB.joinRoom(user.username, roomId);
                socket.join(roomId);
                const message: Message = Admin.createAdminMessage(user.username + " joined the room.", roomId);
                socket.to(roomId).emit("new_message", JSON.stringify(message));
                socket.emit("load_history", JSON.stringify({
                    name: room.name,
                    messages: room.messages
                }));
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
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let status: boolean = false;
        let log_message: LeaveRoomStatus;

        if(user && room) {
            if(user.rooms_joined.includes(roomId)) {
                await profileDB.leaveRoom(user.username, roomId);
                socket.leave(roomId);
                status = true;
                log_message = LeaveRoomStatus.Leave;
                const message: Message = Admin.createAdminMessage( user.username + " left the room.", roomId);
                socket.to(roomId).emit("new_message", JSON.stringify(message));
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

    public async sendMessage(io: SocketIO.Socket, socket: SocketIO.Socket, clientMessage: ClientMessage): Promise<void> {
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        const room: Room | null = await roomDB.getRoom(clientMessage.roomId);
        if(user && room) {
            // message.author.username == user.username
            if(user.rooms_joined.includes(clientMessage.roomId)) {
                const message: Message = {
                    username : user.username,
                    content : clientMessage.content,
                    date : Date.now(),
                    roomId : clientMessage.roomId
                }
                /*await*/ roomDB.addMessage(message);

                io.in(message.roomId).emit("new_message", JSON.stringify(message));
            } else {
                console.log(user.username + " trying to send a message in " + clientMessage.roomId + " that he did not join");
            }
        } else if(user) {
            console.log(user?.username + " trying to send a message in " + clientMessage.roomId + " that does not exist");
        }
    }

    public getUser(socketId: string): PrivateProfile | undefined {
        return this.users.get(socketId);
    }

    private isConnected(username: string): boolean {
        let isConnected: boolean = false;

        this.users.forEach((user: PrivateProfile) => {
            if (user.username === username) {
                isConnected = true;
            }
        });

        return isConnected;
    }

    // Pour deleteChatRoom : Room exists? -> Empty? -> Delete

    /*
    private getChatRoomByName(roomId: string): ChatRoom | undefined {
        return this.chatRooms.find(room => room.name == roomId)
    }

    private getAllUsersChatRooms(user: PublicProfile): ChatRoom[] {
        let chatRooms: ChatRoom[] = [];
        this.chatRooms.forEach((chatRoom) => {
            if(chatRoom.contains(user)) {
                chatRooms.push(chatRoom);
            }
        });
        return chatRooms;
    }
    */

    public updateUser(updatedUser: PrivateProfile): void {
        this.users.forEach((user: PrivateProfile, socketId: string) => {
            if(user.username == updatedUser.username) {
                this.users.set(socketId, updatedUser);
            }
        });
        // emit updated avatar
    }
}