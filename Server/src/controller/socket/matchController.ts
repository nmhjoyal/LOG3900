import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO, OnDisconnect } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import { Trace, Point, GamePreview } from "../../models/drawPoint";

@SocketController()
export default class MatchController {

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

    @OnMessage("trace")
    public start_trace(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() trace: Trace) {
        console.log("start_trace");
        serverHandler.matchHandler.startTrace(io, socket, trace);
    }

    @OnMessage("erase_stroke")
    public erase_stroke(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.matchHandler.eraseStroke(io, socket);
    }

    @OnMessage("erase_point")
    public erase_point(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.matchHandler.erasePoint(io, socket);
    }

    @OnMessage("point")
    public drawTest(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() point: Point) {
        serverHandler.matchHandler.drawTest(io, socket, point);
    }

    @OnMessage("get_drawing")
    public async get_drawing(@SocketIO() io: SocketIO.Server) {
        await serverHandler.matchHandler.getDrawing(io);
    }

    @OnMessage("preview")
    public async preview(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() gamePreview: GamePreview) {
        await serverHandler.matchHandler.preview(socket, gamePreview);
    }
}