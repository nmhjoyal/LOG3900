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
        this.server.signOut(socket);
        console.log("client disconnected");
    }

    @OnMessage("sign_in")
    async sign_in(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() signIn: SignIn) {
        // A tester pas sur que ca fonctionne
        await socket.emit("user_signed_in", this.server.signIn(socket, signIn));
    }

    @OnMessage("sign_out")
    sign_out(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("user_signed_out", this.server.signOut(socket));
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