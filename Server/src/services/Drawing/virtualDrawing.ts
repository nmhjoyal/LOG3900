import { Stroke, GamePreview, Level } from "../../models/drawPoint";
import { Utils } from "./utils";

// 1 virtual drawing per game / per preview
export class VirtualDrawing {

    private roomId: string | null;
    private time: number;
    private timeouts: NodeJS.Timeout[];

    public constructor (roomId: string | null, time: number) {
        this.roomId = roomId;
        // if(roomId) -> Server , else -> Socket
        this.time = time;
        this.timeouts = [];
    }

    public async draw(socketIO: SocketIO.Server | SocketIO.Socket, drawing: Stroke[], level: Level): Promise<void> {
        this.clear(socketIO);
        const speed: number = 250; // points per second
        drawing = Utils.uniform(drawing, speed * this.time);
        let timeStamp: number = 0;
        let deltaT: number = (this.time * 1000) / (Utils.totalPoints(drawing) * Math.pow(2, 2 - level));
        for(let i: number = 0; i < drawing.length; i++) {
            this.timeouts.push(setTimeout(() => {
                if(this.roomId) {
                    socketIO.in(this.roomId).emit("new_stroke", JSON.stringify(drawing[i]));
                } else {
                    socketIO.emit("new_stroke", JSON.stringify(drawing[i]));
                }
            }, timeStamp));
            timeStamp += deltaT;
            for(let j: number = 0; j < drawing[i].StylusPoints.length; j++) {
                this.timeouts.push(setTimeout(() => {
                    console.log(JSON.stringify(drawing[i].StylusPoints[j]));
                    if(this.roomId) {
                        socketIO.in(this.roomId).emit("new_point", JSON.stringify(drawing[i].StylusPoints[j]));
                    } else  {
                        socketIO.emit("new_point", JSON.stringify(drawing[i].StylusPoints[j]));
                    }
                    if(i == drawing.length - 1 && j == drawing[i].StylusPoints.length - 1 && !this.roomId) {
                        socketIO.emit("preview_done");
                    }
                }, timeStamp));
                timeStamp += deltaT;
            }
        }
    }

    public async preview(socket: SocketIO.Socket, gamePreview: GamePreview): Promise<void> {
        Utils.sort(gamePreview.drawing, gamePreview.mode, gamePreview.option);
        await this.draw(socket, gamePreview.drawing, Level.Hard);
    }

    public clear(socketIO: SocketIO.Server | SocketIO.Socket): void {
        if(this.roomId) {
            socketIO.in(this.roomId).emit("new_clear");
        } else {
            socketIO.emit("new_clear");
        }
        this.timeouts.forEach(clearTimeout);
        this.timeouts = [];
    }
}