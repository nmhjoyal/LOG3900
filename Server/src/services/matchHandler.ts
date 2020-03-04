import { MatchMode } from "../models/matchMode";
import DrawPoint from "../models/drawPoint";

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

    public drawTest(io: SocketIO.Server, socket: SocketIO.Socket, point: DrawPoint) {
        if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("drawPoint", point);
        }
    }
}