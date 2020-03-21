import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import DrawPoint from "../../models/drawPoint";
import { CreateMatch, StartMatch } from "../../models/match";

@SocketController()
export default class MatchController {

    /**
     * 
     * Draw test events
     * 
     */
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

    /**
     * 
     * Match events
     * 
     */
    @OnMessage("create_match")
    public async createMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() createMatch: CreateMatch) {
        socket.emit("match_created", JSON.stringify(await serverHandler.createMatch(io, socket, createMatch)));
    }

    @OnMessage("join_match")
    public async joinMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() matchId: string) {
        socket.emit("match_joined", JSON.stringify(await serverHandler.joinMatch(io, socket, matchId)));
    }

    @OnMessage("leave_match")
    public async leaveMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() matchId: string) {
        socket.emit("match_left", JSON.stringify(await serverHandler.leaveMatch(io, socket, matchId)));
    }

    @OnMessage("get_matches")
    public getMatches(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("update_matches", JSON.stringify(serverHandler.getAvailableMatches()));
    }

    @OnMessage("start_match")
    public startMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() startMatch: StartMatch) {
        socket.emit("match_started", JSON.stringify(serverHandler.startMatch(io, socket, startMatch)));
    }
}