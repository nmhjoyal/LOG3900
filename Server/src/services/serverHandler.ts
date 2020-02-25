import SignIn from "../models/signIn";
import ChatRoom from "./chatRoom";
import Message from "../models/message"
import PublicProfile from "../models/publicProfile";
import PrivateProfile from "../models/privateProfile";
import SignInFeedBack from "../models/signInFeedBack";
import { ConnectionStatus } from "../models/signInFeedBack";
import { profileDB } from "../services/Database/profileDB";

export default class ServerHandler {
    private users: Map<string, PublicProfile>; 
    private chatRooms: ChatRoom[];

    public constructor() {
        this.users = new Map();
        this.chatRooms = [];
    }

    public async signIn(socket: SocketIO.Socket, signIn: SignIn): Promise<SignInFeedBack> {
        const privateProfile: PrivateProfile | null = await profileDB.getPrivateProfile(signIn.username);
        let status: boolean = false;
        let log: ConnectionStatus;
        if(privateProfile) {
            if(signIn.password == privateProfile.password) {
                if(this.isConnected(signIn.username)) {
                    log = ConnectionStatus.AlreadyConnected
                } else {
                    const publicProfile: PublicProfile = {
                        username: privateProfile.username,
                        avatar: privateProfile.avatar
                    }
                    this.users.set(socket.id, publicProfile);
                    status = true;
                    log = ConnectionStatus.Connect;
                    console.log(signIn.username + " signed in");
                }
            } else {
                log = ConnectionStatus.InvalidPassword;
            }
        } else {
            log = ConnectionStatus.InvalidUsername;
        }
        const signInFeedback: SignInFeedBack = {
            status: status,
            log: log
        }
        return signInFeedback;
    }

    public signOut(socket: SocketIO.Socket): boolean {
        const user: PublicProfile | undefined = this.users.get(socket.id);
        if (user) {
            this.getAllUsersChatRooms(user.username).forEach((chatRoom) => {
                socket.leave(chatRoom.name);
            });
            console.log("User " + user.username + " signed out")
        } else {
           // To handle: Could not sign out
        }
        return this.users.delete(socket.id);
    }

    public createChatRoom(io: SocketIO.Socket, socket: SocketIO.Socket, roomId: string): void {
        if(this.getChatRoomByName(roomId)) {
            socket.emit("room_already_exists");
        } else {
            this.chatRooms.push(new ChatRoom(roomId));
            io.emit("room_created", roomId);
        }
        console.log(this.chatRooms.toString());
    }

    public joinChatRoom(socket: SocketIO.Socket, roomId: string): void {
        const user: PublicProfile | undefined = this.users.get(socket.id);
        let chatRoom: ChatRoom | undefined = this.getChatRoomByName(roomId);
        if (user && chatRoom) {
            socket.join(roomId);
            socket.to(roomId).emit("user_joined_room", user.username);
            chatRoom.addUser(user);
            socket.emit("load_messages", JSON.stringify(chatRoom.getMessages()));
            console.log(this.chatRooms.toString());
        } else {
            // To handle: Could not join chat room
        }
    }

    public leaveChatRoom(socket: SocketIO.Socket, roomId: string): void {
        socket.leave(roomId);
        const user: PublicProfile | undefined = this.users.get(socket.id);
        const chatRoom: ChatRoom | undefined = this.getChatRoomByName(roomId);
        if (user && chatRoom) {
            chatRoom.removeUser(user);
            socket.to(roomId).emit("user_left_room", user.username);
            console.log(this.chatRooms.toString());
        } else {
            // To handle: Could not leave chat room
        }
    }

    public sendMessage(io: SocketIO.Socket, socket: SocketIO.Socket, roomId: string, message: Message): void {
        message.date = Math.floor(Date.now() / 1000);
        let chatRoom: ChatRoom | undefined = this.getChatRoomByName(roomId);
        if (chatRoom) {
            chatRoom.addMessage(message);
            io.in(roomId).emit("new_message", JSON.stringify(message));
            console.log("*" + message.content + "* has been sent by " + this.users.get(socket.id)?.username + " in " + roomId);
        }
    }

    public getUsers(): Map<string, PublicProfile> {
        return this.users;
    }

    public getUser(socketId: string): PublicProfile | undefined {
        return this.users.get(socketId);
    }

    private isConnected(username: string): boolean {
        let userIsConnected: boolean = false;

        this.users.forEach((value: PublicProfile) => {
            if (value.username === username) {
                userIsConnected = true;
            }
        });

        return userIsConnected;
    }

    private getChatRoomByName(roomId: string): ChatRoom | undefined {
        return this.chatRooms.find(room => room.name == roomId)
    }

    private getAllUsersChatRooms(username: string): ChatRoom[] {
        let chatRooms: ChatRoom[] = [];
        this.chatRooms.forEach((chatRoom) => {
            if(chatRoom.contains(username)) {
                chatRooms.push(chatRoom);
            }
        });
        return chatRooms;
    }
}