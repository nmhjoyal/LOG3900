import { OnConnect, SocketController, ConnectedSocket, OnDisconnect, MessageBody, OnMessage, SocketIO } from "socket-controllers";
import { Room } from "../../services/room";
import { Message } from "../../models/message";
 
@SocketController()
export class MessageController {

    private room: Room;

    public constructor() {
        this.room = new Room("room1");
    }
 
    @OnConnect()
    connection(@SocketIO() io: any, @ConnectedSocket() socket: any) {
        console.log("client connected");
        socket.join(this.room.name);
        socket.to(this.room.name).emit('new_client', socket.id);
    }
 
    @OnDisconnect()
    disconnect(@ConnectedSocket() socket: any) {
        console.log("client disconnected");
        socket.leave(this.room.name);
    }

    @OnMessage("save")
    save(@ConnectedSocket() socket: any, @MessageBody() message: any) {
        console.log("received message:", message);
        console.log("setting id to the message and sending it back to the client");
        message.id = 1;
        socket.emit("message_saved", message);
    }

    @OnMessage("test")
    test(@ConnectedSocket() socket: any) {
        console.log("test received");
    }

    @OnMessage("send_message")
    send_message(@SocketIO() io: any, @ConnectedSocket() socket: any, @MessageBody() message: Message) {
        console.log("*" + message.content + "* has been sent by " + socket.id);
        this.room.addMessage(message);
        io.in(this.room.name).emit("new_message", JSON.stringify(message));
        
    }
}

// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme