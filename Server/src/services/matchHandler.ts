import { MatchMode } from "../models/matchMode";
import { GamePreview, Stroke, StylusPoint, Game } from "../models/drawPoint";
import { VirtualDrawing } from "./Drawing/virtualDrawing";
import { gameDB } from "./Database/gameDB";
// import { VirtualPlayer } from "./Drawing/virtualPlayer";

export default class MatchHandler {
    // private currentMatches: Match[];

    // TEMPORARY
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids
    private top: number;
    private previews: VirtualDrawing[];

    public constructor() {
        // this.currentMatches = new Array<Match>();
        this.observers = [];
        this.top = 0;
        this.previews = [];
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
            socket.to("freeDrawRoomTest").emit("new_stroke", JSON.stringify(stroke));
        // }
    }

    public eraseStroke(io: SocketIO.Server, socket: SocketIO.Socket): void {
        socket.to("freeDrawRoomTest").emit("new_erase_stroke");
    }

    public erasePoint(io: SocketIO.Server, socket: SocketIO.Socket): void {
        socket.to("freeDrawRoomTest").emit("new_erase_point");
    }

    public point(io: SocketIO.Server, socket: SocketIO.Socket, point: StylusPoint): void {
        // if (socket.id == this.drawer) {
        socket.to("freeDrawRoomTest").emit("new_point", JSON.stringify(point));
        // }
    }

    public clear(io: SocketIO.Server, socket: SocketIO.Socket): void {
        // Pour preview seulement
        let virtualDrawing: VirtualDrawing | undefined = this.previews.find(drawing => socket.id == drawing.getId());
        if(virtualDrawing) {
            console.log("clear");
            virtualDrawing.clear();
        };
    }

    public async getDrawing(io: SocketIO.Server): Promise<void> {
        /*
        const game: Game = await gameDB.getRandomGame();
        let virtualDrawing: VirtualDrawing | undefined = this.previews.find(drawing => "freeDrawRoomTest" == drawing.getId());
        if(!virtualDrawing) {
            virtualDrawing = new VirtualDrawing("froomDrawRoomTest", io, 20);
            this.previews.push(virtualDrawing);
        }
        virtualDrawing.draw(game.drawing, game.level);
        */
    }

    // previewHandler.ts
    public async preview(socket: SocketIO.Socket, gamePreview: GamePreview): Promise<void> {
        let virtualDrawing: VirtualDrawing | undefined = this.previews.find(drawing => socket.id == drawing.getId());
        if(!virtualDrawing) {
            virtualDrawing = new VirtualDrawing(null, socket, 7.5);
            this.previews.push(virtualDrawing);
        }
        virtualDrawing.preview(gamePreview);
    }
}