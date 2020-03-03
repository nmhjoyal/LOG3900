import ChatHandler from "../../services/chatHandler";
import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import { ClientMessage } from "../../models/message";

@SocketController()
export class ChatController {

    private chatHandler: ChatHandler;

    public constructor() {
        this.chatHandler = new ChatHandler();
    }

    @OnMessage("create_chat_room")
    public async create_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("room_created", JSON.stringify(await this.chatHandler.createChatRoom(socket, roomId)));
    }

    @OnMessage("delete_chat_room")
    public async delete_chat_room(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("room_deleted", JSON.stringify(await this.chatHandler.deleteChatRoom(io, socket, roomId)));
    }

    @OnMessage("join_chat_room")
    public async join_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("user_joined_room", JSON.stringify(await this.chatHandler.joinChatRoom(socket, roomId)));
    }

    @OnMessage("leave_chat_room")
    public async leave_chat_room(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("user_left_room", JSON.stringify(await this.chatHandler.leaveChatRoom(socket, roomId)));
    }
    
    @OnMessage("send_message")
    public async send_message(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() message: ClientMessage) {
        await this.chatHandler.sendMessage(io, socket, message);
    }
}