import { OnConnect, SocketController, ConnectedSocket, OnDisconnect, MessageBody, OnMessage, SocketIO } from "socket-controllers";
import { ServerHandler } from "../../services/serverHandler";
import Message from "../../models/message";
import User from "../../models/user";
 
@SocketController()
export class SocketProtoController {

    private server: ServerHandler;

    public constructor() {
        this.server = new ServerHandler("room1");
    }
 
    @OnConnect()
    connection(@ConnectedSocket() socket: any) {
        console.log("user connected");
        // socket.join(this.server.name);
    }
 
    @OnDisconnect()
    disconnect(@ConnectedSocket() socket: SocketIO.Socket) {
        console.log("user disconnected");
        socket.leave(this.server.name);
    }

    @OnMessage("send_message")
    send_message(@SocketIO() io: SocketIO.Socket, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() message: Message) {
        console.log("*" + message.content + "* has been sent by " + this.server.getUser(socket.id)?.username);
        io.in(this.server.name).emit("new_message", JSON.stringify(message));
    }

    @OnMessage("sign_in")
    sign_in(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() user: User) {
        console.log(user.username + " signed in");
        socket.emit("user_signed_in", JSON.stringify(this.server.signIn(socket.id, user)));
    }

    @OnMessage("sign_out")
    sign_out(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("user_signed_out", JSON.stringify(this.server.signOut(socket.id)));
    }

    @OnMessage("join_chat_room")
    join_chat_room(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.join(this.server.name);
        socket.to(this.server.name).emit("new_user", this.server.getUser(socket.id)?.username);
    }

    @OnMessage("leave_chat_room")
    leave_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() user: User) {
        socket.leave(this.server.name);
        socket.to(this.server.name).emit("new_user", socket.id);
    }

}

// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme