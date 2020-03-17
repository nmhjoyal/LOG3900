import { Game, Stroke, Mode, Color, Trace, Point, GamePreview, Level } from "../../models/drawPoint";

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
        // Set tops
        let top: number = 0;
        gamePreview.drawing.forEach((stroke: Stroke) => {
            stroke.DrawingAttributes.Top = top++;
        });
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

    private async classic(drawing: Stroke[], level: number): Promise<void> {
        let totalPoints: number = 0;
        drawing.forEach((line: Stroke) => {
            totalPoints += line.StylusPoints.length;
        });

        for(let stroke of drawing) {
            if(this.roomId) {
                console.log("new stroke");
                this.io.in(this.roomId).emit("new_stroke", JSON.stringify(stroke));
            } else {
                this.io.emit("new_stroke", JSON.stringify(stroke));
            }
            for(let stylusPoint of stroke.StylusPoints) {
                await VirtualPlayer.delay(this.timePerRound * 1000 / totalPoints);
                if(this.roomId) {
                    this.io.in(this.roomId).emit("new_point", JSON.stringify(stylusPoint));
                } else  {
                    this.io.emit("new_point", JSON.stringify(stylusPoint));
                }
            }
        }
    }

    private async random(game: Stroke[], level: number): Promise<void> {

    }

    private async panoramic(game: Stroke[], level: number): Promise<void> {

    }

    private async centered(game: Stroke[], level: number): Promise<void> {

    }

    private static async delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}