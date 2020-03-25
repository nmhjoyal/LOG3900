import { Stroke, GamePreview, Level } from "../../models/drawPoint";
import { Utils } from "./utils";

// 1 virtual drawing per game / per preview
export class VirtualDrawing {

    private roomId: string | null;
    private io: SocketIO.Server | SocketIO.Socket;
    private time: number;
    private timeouts: NodeJS.Timeout[];

    public constructor (roomId: string | null, io: SocketIO.Server | SocketIO.Socket, time: number) {
        this.roomId = roomId;
        // if(roomId) -> Server , else -> Socket
        this.io = io;
        this.time = time;
        this.timeouts = [];
    }

    public async draw(drawing: Stroke[], level: Level): Promise<void> {
        let start: number = Date.now();
        this.clear();
        let timeStamp: number = 0;
        let deltaT: number = (this.time * 1000) / (this.totalPoints(drawing) * Math.pow(2, 2 - level));
        for(let i: number = 0; i < drawing.length; i++) {
            this.timeouts.push(setTimeout(() => {
                console.log("stroke " + i + " time " + (Date.now() - start));
                if(this.roomId) {
                    // Envoyer uniquement le premier point de la stroke
                    this.io.in(this.roomId).emit("new_stroke", JSON.stringify(drawing[i]));
                } else {
                    this.io.emit("new_stroke", JSON.stringify(drawing[i]));
                }
            }, timeStamp));
            for(let j: number = 0; j < drawing[i].StylusPoints.length; j++) {
                this.timeouts.push(setTimeout(() => {
                    console.log("stroke " + i + " point " + j + " time " + (Date.now() - start));
                    if(this.roomId) {
                        this.io.in(this.roomId).emit("new_point", JSON.stringify(drawing[i].StylusPoints[j]));
                    } else  {
                        this.io.emit("new_point", JSON.stringify(drawing[i].StylusPoints[j]));
                    }
                }, timeStamp));
                timeStamp += deltaT;
            }
        }
        console.log(Date.now() - start);
    }

    public async preview(gamePreview: GamePreview) {
        Utils.sort(gamePreview.drawing, gamePreview.mode, gamePreview.option);
        await this.draw(gamePreview.drawing, Level.Hard);
    }

    private totalPoints(drawing: Stroke[]): number {
        let totalPoints: number = 0;
        drawing.forEach((line: Stroke) => {
            totalPoints += line.StylusPoints.length;
        });
        return totalPoints;
    }

    public clear(): void {
        if(this.roomId) {
            this.io.in(this.roomId).emit("new_clear");
        } else {
            this.io.emit("new_clear");
        }
        for(const timeout of this.timeouts) {
            clearTimeout(timeout);
        }
        this.timeouts = [];
    }

    public getId(): string {
        if(this.roomId) {
            return this.roomId;
        } else {
            return (this.io as SocketIO.Socket).id;
        }
    }
}