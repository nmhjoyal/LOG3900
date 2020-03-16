import { Game, Line, Mode, Color, Trace, Point, GamePreview, Level } from "../../models/drawPoint";

export class VirtualPlayer {

    public username: string;
    private readonly previewTime = 10;
    private roomId: string | null;
    private io: SocketIO.Server | SocketIO.Socket;
    private timePerRound: number;

    public constructor (username: string, roomId: string | null, io: SocketIO.Server | SocketIO.Socket) {
        this.username = username;
        this.roomId = roomId;
        // if(roomId) -> Server , else -> Socket
        this.io = io;
        this.timePerRound = 0;
    }

    public setTimePerRound(timePerRound: number) {
        this.timePerRound = timePerRound;
    }

    public async draw(game: Game): Promise<void> {
        switch(game.mode) {
            case Mode.Classic :
                await this.classic(game.drawing, game.level);
                break;
            case Mode.Random :
                await this.random(game.drawing, game.level);
                break;
            case Mode.Panoramic :
                await this.panoramic(game.drawing, game.level);
                break;
            case Mode.Centered :
                await this.centered(game.drawing, game.level);
                break;
        }
    }

    public async preview(gamePreview: GamePreview) {
        this.setTimePerRound(this.previewTime);
        switch(gamePreview.mode) {
            case Mode.Classic :
                await this.classic(gamePreview.drawing, Level.Hard);
                break;
            case Mode.Random :
                await this.random(gamePreview.drawing, Level.Hard);
                break;
            case Mode.Panoramic :
                await this.panoramic(gamePreview.drawing, Level.Hard);
                break;
            case Mode.Centered :
                await this.centered(gamePreview.drawing, Level.Hard);
                break;
        }
    }

    private async classic(drawing: Line[], level: number): Promise<void> {
        let totalPoints: number = 0;
        drawing.forEach((line: Line) => {
            totalPoints += line.StylusPoints.length;
        });

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
            if(this.roomId) {
                this.io.in(this.roomId).emit("new_trace", JSON.stringify(trace));
            } else {
                this.io.emit("new_trace", JSON.stringify(trace));
            }
            for(let stylusPoint of line.StylusPoints) {
                const point: Point = {
                    x: stylusPoint.X,
                    y: stylusPoint.Y
                }
                await VirtualPlayer.delay(this.timePerRound * 1000 / totalPoints);
                if(this.roomId) {
                    this.io.in(this.roomId).emit("new_point", JSON.stringify(point));
                } else  {
                    this.io.emit("new_point", JSON.stringify(point));
                }
            }
        }
    }

    private async random(game: Line[], level: number): Promise<void> {

    }

    private async panoramic(game: Line[], level: number): Promise<void> {

    }

    private async centered(game: Line[], level: number): Promise<void> {

    }

    private static async delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}