import { OnConnect, SocketController, ConnectedSocket, OnDisconnect, MessageBody, OnMessage, SocketIO } from "socket-controllers";
import ServerHandler from "../../services/serverHandler";
import Message from "../../models/message";
import User from "../../models/user";
 
@SocketController()
export class SocketProtoController {

    private server: ServerHandler;

    public constructor() {
        this.server = new ServerHandler("room1");
    }
 
    @OnConnect()
    connection() {
        console.log("client connected");
    }
 
    @OnDisconnect()
    disconnect(@ConnectedSocket() socket: any) {
        this.server.signOutUser(socket.id);
        console.log("client disconnected");
    }

    @OnMessage("test")
    messageTest() {
        console.log("Hi")
    }

    @OnMessage("send_message")
    send_message(@SocketIO() io: any, @ConnectedSocket() socket: any, @MessageBody() message: Message) {
        console.log("*" + message.content + "* has been sent by " + socket.id);
        io.in(this.server.name).emit("new_message", JSON.stringify(message));
    }

    @OnMessage("sign_in")
    sign_in(@ConnectedSocket() socket: any, @MessageBody() user: User) {
        console.log("User " + user.username + " signed in on socket id : " + socket.id)
        let cantConnect:boolean =  this.server.signInUser(socket.id, user);
        console.log(cantConnect);
        socket.emit("user_signed_in", JSON.stringify(cantConnect));
    }

    @OnMessage("sign_out")
    sign_out(@ConnectedSocket() socket: any) {
        this.server.signOutUser(socket.id);
        socket.leave(this.server.name);
        socket.emit("user_signed_out", JSON.stringify(this.server.signOutUser(socket.id)));
    }

    @OnMessage("join_chat_room")
    join_chat_room(@ConnectedSocket() socket: any) {
        console.log(this.server.name + "joined")
        // Eventually, this.server.joinRoom()
        socket.join(this.server.name);
        socket.to(this.server.name).emit("new_client", this.server.getUser(socket.id)?.username);
    }

    @OnMessage("leave_chat_room")
    leave_chat_room(@ConnectedSocket() socket: any) {
        socket.leave(this.server.name);
        socket.to(this.server.name).emit("new_client", this.server.getUser(socket.id)?.username);
    }

}

// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme