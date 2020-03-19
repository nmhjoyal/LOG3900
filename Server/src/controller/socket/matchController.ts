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
    public async createMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() createMatch: CreateMatch) {
        socket.emit("match_created", await serverHandler.createMatch(io, socket, createMatch));
    }

    @OnMessage("join_match")
    public async joinMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() matchId: string) {
        socket.emit("match_joined", await serverHandler.joinMatch(io, socket, matchId));
    }

    @OnMessage("leave_match")
    public async leaveMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() matchId: string) {
        socket.emit("match_left", await serverHandler.leaveMatch(io, socket, matchId));
    }

}