import { MatchMode } from "../models/matchMode";
import { Game, GamePreview, Stroke, StylusPoint, ScreenResolution } from "../models/drawPoint";
import { VirtualPlayer } from "./Drawing/virtualPlayer";
import { gameDB } from "./Database/gameDB";
// import { VirtualPlayer } from "./Drawing/virtualPlayer";

export default class MatchHandler {
    // private currentMatches: Match[];

    // TEMPORARY
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids
    private top: number;

    public constructor() {
        // this.currentMatches = new Array<Match>();
        this.observers = [];
        this.top = 0;
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

    public stroke(io: SocketIO.Server, socket: SocketIO.Socket, stroke: Stroke): void {
        // if (socket.id == this.drawer) {
            stroke.DrawingAttributes.Top = this.top++;
            console.log(stroke.DrawingAttributes);
            socket.to("freeDrawRoomTest").emit("new_stroke", JSON.stringify(stroke));
        // }
    }

    public eraseStroke(io: SocketIO.Server, socket: SocketIO.Socket): void {
        console.log("erase stroke");
        socket.to("freeDrawRoomTest").emit("new_erase_stroke");
    }

    public erasePoint(io: SocketIO.Server, socket: SocketIO.Socket): void {
        console.log("erase point");
        socket.to("freeDrawRoomTest").emit("new_erase_point");
    }

    public point(io: SocketIO.Server, socket: SocketIO.Socket, point: StylusPoint): void {
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