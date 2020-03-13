import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import DrawPoint from "../../models/drawPoint";
import { CreateMatch } from "../../models/match";

@SocketController()
export default class MatchController {

    @OnMessage("connect_free_draw")
    public connect_free_draw(@ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.matchHandler.enterFreeDrawTestRoom(socket);
    }

    @OnMessage("disconnect_free_draw")
    public disconnect_free_draw(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.matchHandler.leaveFreeDrawTestRoom(io, socket);
    }

    @OnMessage("drawTest")
    public drawTest(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() drawPoint: DrawPoint) {
        serverHandler.matchHandler.drawTest(io, socket, drawPoint);
    }

    @OnMessage("create_match")
    public createMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() createMatch: CreateMatch) {
        io.emit("match_created", serverHandler.matchHandler.createMatch(socket.id, createMatch));
    }

    @OnMessage("join_match")
    public joinMatch(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() matchId: string) {
        socket.emit("match_joined", serverHandler.matchHandler.joinMatch(socket, matchId));
    }

}