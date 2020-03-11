import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO, OnDisconnect } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import DrawPoint from "../../models/drawPoint";

@SocketController()
export default class ChatController {

    @OnMessage("connect_free_draw")
    public async connect_free_draw(@ConnectedSocket() socket: SocketIO.Socket) {
        console.log("connected to free draw");
        serverHandler.matchHandler.enterFreeDrawTestRoom(socket);
    }

    @OnDisconnect()
    public disconnect(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        console.log("disconnected to free draw");
        serverHandler.matchHandler.leaveFreeDrawTestRoom(io, socket);
    }

    @OnMessage("disconnect_free_draw")
    public async disconnect_free_draw(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        console.log("disconnected to free draw");
        serverHandler.matchHandler.leaveFreeDrawTestRoom(io, socket);
    }

    @OnMessage("drawTest")
    public drawTest(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() drawPoint: DrawPoint) {
        serverHandler.matchHandler.drawTest(io, socket, drawPoint);
    }
}