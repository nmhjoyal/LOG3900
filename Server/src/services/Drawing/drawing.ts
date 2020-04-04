import { Stroke, StylusPoint } from "../../models/drawPoint";

export class Drawing {

    private roomId: string;
    private top: number;

    public constructor(roomId: string) {
        this.roomId = roomId;
        this.top = 0;
    }

    public stroke(socket: SocketIO.Socket, stroke: Stroke): void {
        stroke.DrawingAttributes.Top = this.top++;
        socket.to(this.roomId).emit("new_stroke", JSON.stringify(stroke));
    }

    public eraseStroke(socket: SocketIO.Socket): void {
        socket.to(this.roomId).emit("new_erase_stroke");
    }

    public erasePoint(socket: SocketIO.Socket): void {
        socket.to(this.roomId).emit("new_erase_point");
    }

    public point(socket: SocketIO.Socket, point: StylusPoint): void {
        socket.to(this.roomId).emit("new_point", JSON.stringify(point));
    }

    public clear(socket: SocketIO.Socket): void {
        socket.to(this.roomId).emit("new_clear");
    }

    public reset(io: SocketIO.Server): void {
        io.in(this.roomId).emit("new_clear");
    }
}