import { MatchMode } from "../models/matchMode";
import { Trace, Point, Game, GamePreview, ScreenResolution } from "../models/drawPoint";
import { VirtualPlayer } from "./Drawing/virtualPlayer";
import { gameDB } from "./Database/gameDB";
// import { VirtualPlayer } from "./Drawing/virtualPlayer";

export default class MatchHandler {
    // private currentMatches: Match[];

    // TEMPORARY
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids

    public constructor() {
        // this.currentMatches = new Array<Match>();
        this.observers = [];
    }

    public startMatch(matchMode: MatchMode) {
        // this.currentMatches.push(MatchInstance.getMatchClassInstance(matchMode));
    }


    // TEMPORARY
    public enterFreeDrawTestRoom(socket: SocketIO.Socket): void {
        if (this.drawer) {
            this.observers.push(socket.id);
            socket.emit("observer");
        } else {
            this.drawer = socket.id;
            socket.emit("drawer");
        }
        socket.join("freeDrawRoomTest");
    }

    public leaveFreeDrawTestRoom(io: SocketIO.Server, socket: SocketIO.Socket): void {
        if (socket.id == this.drawer) {
            const newDrawer: string | undefined = this.observers.pop();
            if (newDrawer) {
                this.drawer = newDrawer;
                io.to(this.drawer).emit("drawer");
            }
        } else {
            this.observers.splice(this.observers.indexOf(socket.id), 1);
        }
        socket.leave("freeDrawRoomTest");
    }

    public startTrace(io: SocketIO.Server, socket: SocketIO.Socket, trace: Trace): void {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("new_trace", JSON.stringify(trace));
        // }
    }

    public drawTest(io: SocketIO.Server, socket: SocketIO.Socket, point: Point): void {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("new_point", JSON.stringify(point));
        // }
    }

    public scaleViews(io: SocketIO.Server, socket: SocketIO.Socket, screen: ScreenResolution): void {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("scale_view", JSON.stringify(screen));
            console.log("views_scaled")
        // }
    }

    public async getDrawing(io: SocketIO.Server): Promise<void> {
        const game: Game = await gameDB.getRandomGame();
        console.log(JSON.stringify(game));
        const virtualPlayer: VirtualPlayer = new VirtualPlayer("bot", "freeDrawRoomTest", io);
        virtualPlayer.setTimePerRound(10);
        virtualPlayer.draw(game);
    }

    // previewHandler.ts
    public async preview(socket: SocketIO.Socket, gamePreview: GamePreview): Promise<void> {
        const virtualPlayer: VirtualPlayer = new VirtualPlayer("bot", null, socket);
        virtualPlayer.setTimePerRound(5);
        virtualPlayer.preview(gamePreview);
    }
}