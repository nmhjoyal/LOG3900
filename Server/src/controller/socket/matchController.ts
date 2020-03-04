import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import DrawPoint from "../../models/drawPoint";

@SocketController()
export default class ChatController {

    @OnMessage("connect_free_draw")
    public async connect_free_draw(@ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.matchHandler.enterFreeDrawTestRoom(socket);
    }

    @OnMessage("disconnect_free_draw")
    public async disconnect_free_draw(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.matchHandler.leaveFreeDrawTestRoom(io, socket);
    }

    @OnMessage("drawTest")
    public async drawTest(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() drawPoint: DrawPoint) {
        serverHandler.matchHandler.drawTest(io, socket, drawPoint);
    }

}