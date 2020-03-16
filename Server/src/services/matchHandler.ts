import { MatchMode } from "../models/matchMode";
import { Trace, Line, Color } from "../models/drawPoint";
import Point from "../models/drawPoint";

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

    public startTrace(io: SocketIO.Server, socket: SocketIO.Socket, trace: Trace) {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("new_trace", JSON.stringify(trace));
        // }
    }

    public drawTest(io: SocketIO.Server, socket: SocketIO.Socket, point: Point) {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("drawPoint", JSON.stringify(point));
        // }
    }

    public async sendDrawing(socket: SocketIO.Socket, drawing: Line[]): Promise<void> {
        for(let line of drawing) {
            const color: Color = {
                r: parseInt(line.DrawingAttributes.Color.substring(3, 5), 16),
                g: parseInt(line.DrawingAttributes.Color.substring(5, 7), 16),
                b: parseInt(line.DrawingAttributes.Color.substring(7, 9), 16)
            }
            const startPoint: Point = {
                x: line.StylusPoints[0].X,
                y: line.StylusPoints[0].Y
            }
            const trace: Trace = {
                color: color,
                point: startPoint,
                width: line.DrawingAttributes.Width,
                tool: "crayon"
            }
            await this.delay(50);
            socket.to("freeDrawRoomTest").emit("new_trace", JSON.stringify(trace));
            for(let stylusPoint of line.StylusPoints) {
                const point: Point = {
                    x: stylusPoint.X,
                    y: stylusPoint.Y
                }
                await this.delay(50);
                socket.to("freeDrawRoomTest").emit("drawPoint", JSON.stringify(point));
            }
        }
    }

    private async delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}