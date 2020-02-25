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
        socket.emit("user_signed_in", await this.server.signIn(socket, signIn));
    }

    @OnMessage("sign_out")
    public sign_out(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("user_signed_out", this.server.signOut(socket));
    }

    @OnMessage("create_chat_room")
    public create_chat_room(@SocketIO() io: SocketIO.Socket, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        this.server.createChatRoom(io, socket, roomId);
    }

    @OnMessage("join_chat_room")
    public join_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        this.server.joinChatRoom(socket, roomId);
    }

    @OnMessage("leave_chat_room")
    public leave_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        this.server.leaveChatRoom(socket, roomId);
    }

    
    @OnMessage("send_message")
    public send_message(@SocketIO() io: SocketIO.Socket, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string, @MessageBody() message: Message) {
        this.server.sendMessage(io, socket, roomId, message);
    }

}

// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme