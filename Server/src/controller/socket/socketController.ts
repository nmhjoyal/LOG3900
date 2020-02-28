import { OnConnect, SocketController, ConnectedSocket, OnDisconnect, MessageBody, OnMessage, SocketIO } from "socket-controllers";
import ServerHandler from "../../services/serverHandler";
import Message from "../../models/message";
import SignIn from "../../models/signIn";
 
@SocketController()
export class SocketProtoController {

    private server: ServerHandler;

    public constructor() {
        this.server = new ServerHandler();
    }
 
    @OnConnect()
    public connection(@ConnectedSocket() socket: SocketIO.Socket) {
        console.log("client connected");
    }
 
    @OnDisconnect()
    public disconnect(@ConnectedSocket() socket: SocketIO.Socket) {
        this.server.signOut(socket);
        console.log("client disconnected");
    }

    @OnMessage("sign_in")
    public async sign_in(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() signIn: SignIn) {
        socket.emit("user_signed_in", JSON.stringify(await this.server.signIn(socket, signIn)));
    }

    @OnMessage("sign_out")
    public async sign_out(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("user_signed_out", JSON.stringify(await this.server.signOut(socket)));
    }

    @OnMessage("create_chat_room")
    public async create_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("room_created", JSON.stringify(await this.server.createChatRoom(roomId)));
    }

    @OnMessage("join_chat_room")
    public async join_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("user_joined_room", JSON.stringify(await this.server.joinChatRoom(socket, roomId)));
    }

    @OnMessage("leave_chat_room")
    public async leave_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("user_left_room", JSON.stringify(await this.server.leaveChatRoom(socket, roomId)));
    }
    
    @OnMessage("send_message")
    public async send_message(@SocketIO() io: SocketIO.Socket, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() message: Message) {
        await this.server.sendMessage(io, socket, message);
    }
}

// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme