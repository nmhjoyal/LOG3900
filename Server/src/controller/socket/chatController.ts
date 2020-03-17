import { serverHandler } from "../../services/serverHandler";
import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import { ClientMessage } from "../../models/message";
import { CreateRoom, Invitation } from "../../models/room";
import { roomDB } from "../../services/Database/roomDB";

@SocketController()
export default class ChatController {

    @OnMessage("create_chat_room")
    public async create_chat_room(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() room: CreateRoom) {
        socket.emit("room_created", JSON.stringify(await serverHandler.chatHandler.createChatRoom(io, socket, room)));
    }

    @OnMessage("delete_chat_room")
    public async delete_chat_room(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("room_deleted", JSON.stringify(await serverHandler.chatHandler.deleteChatRoom(io, socket, roomId)));
    }

    @OnMessage("join_chat_room")
    public async join_chat_room(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("user_joined_room", JSON.stringify(await serverHandler.chatHandler.joinChatRoom(io, socket, roomId)));
    }

    @OnMessage("leave_chat_room")
    public async leave_chat_room(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
        socket.emit("user_left_room", JSON.stringify(await serverHandler.chatHandler.leaveChatRoom(io, socket, roomId)));
    }

    @OnMessage("send_invite")
    public async invite_chat_room(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() invitation: Invitation) {
        socket.emit("user_sent_invite", JSON.stringify(await serverHandler.chatHandler.invite(io, socket, invitation)));
    }
    
    @OnMessage("send_message")
    public async send_message(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() message: ClientMessage) {
        await serverHandler.sendMessage(io, socket, message);
    }

    @OnMessage("get_rooms")
    public async getRooms(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("rooms_retrived", await roomDB.getRooms());
    }
}