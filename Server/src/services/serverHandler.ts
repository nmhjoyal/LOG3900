import User from "../models/user";
import ChatRoom from "./chatRoom";
import Message from "../models/message"

export default class ServerHandler {
    private users: Map<string, User>;
    // TEMPORARY : eventually array of rooms
    private chatRooms: ChatRoom[];

    public constructor() {
        this.users = new Map();
        this.chatRooms = [new ChatRoom("Room1")];
    }

    /**
     * Returns true if user is signed in
     * @param user user we wish to add
     */
    public signIn(socketId: string, user: User): boolean {
        let canSignIn: boolean = false;

        if (!this.isConnected(user.username)) {
            console.log("User " + user.username + " signed in")
            this.users.set(socketId, user);
            canSignIn = true;
        }

        return canSignIn;
    }

    public signOut(socketId: string): boolean {
        let user: User | undefined = this.getUser(socketId);
        if (user) {
            console.log("User " + user.username + " signed out")
        }

        return this.users.delete(socketId);
    }

    public getUsers(): Map<string, User> {
        return this.users;
    }

    public getUser(socketId: string): User | undefined {
        return this.users.get(socketId);
    }

    public createChatRoom(io: SocketIO.Socket, socket: SocketIO.Socket, roomId: string): void {
        if(this.getChatRoomByName(roomId) == undefined) {
            this.chatRooms.push(new ChatRoom(roomId));
            io.emit("room_created", roomId);
        } else {
            socket.emit("room_already_exists");
        }
        console.log(this.chatRooms.toString());
    }

    public joinChatRoom(socket: SocketIO.Socket, roomId: string): void {
        let user: User | undefined = this.users.get(socket.id);
        let chatRoom: ChatRoom | undefined = this.getChatRoomByName(roomId);
        if (user && chatRoom) {
            socket.join(roomId);
            socket.to(roomId).emit("user_joined", this.getUser(socket.id)?.username);
            chatRoom.addUser(user);
            socket.emit("load_messages", JSON.stringify(chatRoom.getMessages));
            console.log(this.chatRooms.toString());
        }
    }

    public leaveChatRoom(socket: SocketIO.Socket, roomId: string): void {
        socket.leave(roomId);
        let user: User | undefined = this.users.get(socket.id);
        if (user) {
            this.getChatRoomByName(roomId)?.removeUser(user);
        }
        socket.to(roomId).emit("user_left", this.getUser(socket.id)?.username);
        console.log(this.chatRooms.toString());
    }

    public sendMessage(io: SocketIO.Socket, socket: SocketIO.Socket, roomId: string, message: Message): void{
        message.date = Math.floor(Date.now() / 1000);
        let chatRoom: ChatRoom | undefined = this.getChatRoomByName(roomId);
        if (chatRoom) {
            chatRoom.addMessage(message);
            io.in(roomId).emit("new_message", JSON.stringify(message));
            console.log("*" + message.content + "* has been sent by " + this.getUser(socket.id)?.username + " in " + roomId);
        }
    }

    private isConnected(username: string): boolean {
        let userIsConnected: boolean = false;

        this.users.forEach((value: User) => {
            if (value.username === username) {
                userIsConnected = true;
            }
        });

        return userIsConnected;
    }

    private getChatRoomByName(roomId: string): ChatRoom | undefined {
        return this.chatRooms.find(room => room.name == roomId)
    }
}