import { OnMessage, SocketController, MessageBody, ConnectedSocket, SocketIO } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import { CreateMatch } from "../../models/match";
import { GamePreview, Stroke, StylusPoint } from "../../models/drawPoint";

@SocketController()
export default class MatchController {

    /**
     * 
     * Draw events
     * 
     */
    @OnMessage("stroke")
    public stroke(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() stroke: Stroke) {
        serverHandler.stroke(socket, stroke);
    }

    @OnMessage("point")
    public point(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() point: StylusPoint) {
        serverHandler.point(socket, point);
    }

    @OnMessage("erase_stroke")
    public erase_stroke(@ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.eraseStroke(socket);
    }

    @OnMessage("erase_point")
    public erase_point(@ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.erasePoint(socket);
    }

    @OnMessage("clear")
    public clear(@ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.clear(socket);
    }

    /**
     * 
     * Preview
     * 
     */
    @OnMessage("preview")
    public async preview(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() gamePreview: GamePreview) {
        await serverHandler.matchHandler.preview(socket, gamePreview);
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
    public async leaveMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("match_left", JSON.stringify(await serverHandler.leaveMatch(io, socket)));
    }

    @OnMessage("get_matches")
    public getMatches(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("update_matches", JSON.stringify(serverHandler.matchHandler.getAvailableMatches()));
    }

    @OnMessage("start_match")
    public startMatch(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.startMatch(io, socket);
    }

    @OnMessage("add_vp")
    public addVirtualPlayer(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("vp_added", JSON.stringify(serverHandler.addVirtualPlayer(io, socket)));
    }

    @OnMessage("remove_vp")
    public removeVirtualPlayer(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("vp_removed", JSON.stringify(serverHandler.removeVirtualPlayer(io, socket)));
    }

    @OnMessage("get_players")
    public getPlayers(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() matchId: string) {
        socket.emit("update_players", JSON.stringify(serverHandler.matchHandler.getPlayers(matchId)));
    }

    @OnMessage("start_turn")
    public start_turn(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() word: string) {
        serverHandler.startTurn(io, socket, word);
    }

    @OnMessage("guess")
    public guess(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() guess: string) {
        serverHandler.guess(io, socket, guess);
    }

    @OnMessage("hint")
    public hint(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.hint(io, socket);
    }
}