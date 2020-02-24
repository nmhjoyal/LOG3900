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
    connection(@ConnectedSocket() socket: SocketIO.Socket) {
        console.log("client connected");
    }
 
    @OnDisconnect()
    disconnect(@ConnectedSocket() socket: SocketIO.Socket) {
        this.server.signOut(socket.id);
        console.log("client disconnected");
    }

    @OnMessage("sign_in")
    sign_in(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() signIn: SignIn) {
        let canConnect:boolean =  this.server.signIn(socket.id, signIn);
        socket.emit("user_signed_in", JSON.stringify(canConnect));
    }

    @OnMessage("sign_out")
    sign_out(@ConnectedSocket() socket: SocketIO.Socket) {
        this.server.signOut(socket.id);
        // Quitter toutes les rooms dans lequel le user est
        socket.emit("user_signed_out", JSON.stringify(this.server.signOut(socket.id)));
    }

    @OnMessage("create_chat_room")
    create_chat_room(@SocketIO() io: SocketIO.Socket, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        this.server.createChatRoom(io, socket, roomId);
    }

    @OnMessage("join_chat_room")
    join_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        this.server.joinChatRoom(socket, roomId);
    }

    @OnMessage("leave_chat_room")
    leave_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        this.server.leaveChatRoom(socket, roomId);
    }

    
    @OnMessage("send_message")
    send_message(@SocketIO() io: SocketIO.Socket, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string, @MessageBody() message: Message) {
        this.server.sendMessage(io, socket, roomId, message);
    }

}

// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme