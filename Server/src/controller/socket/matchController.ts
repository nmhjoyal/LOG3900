import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import MatchHandler from "../../services/matchHandler";
import DrawPoint from "../../models/drawPoint";

@SocketController()
export default class ChatController {
    private matchHandler: MatchHandler;

    public constructor() {
        this.matchHandler = new MatchHandler();
    }

    @OnMessage("connect_free_draw")
    public async connect_free_draw(@ConnectedSocket() socket: SocketIO.Socket) {
        this.matchHandler.enterFreeDrawTestRoom(socket);
    }

    @OnMessage("disconnect_free_draw")
    public async disconnect_free_draw(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        this.matchHandler.leaveFreeDrawTestRoom(io, socket);
    }

    @OnMessage("drawTest")
    public async drawTest(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() drawPoint: DrawPoint) {
        this.matchHandler.drawTest(io, socket, drawPoint);
    }

}