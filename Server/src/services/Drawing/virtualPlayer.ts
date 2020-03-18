import { Game, Stroke, Mode, Color, Trace, Point, GamePreview, Level, StylusPoint } from "../../models/drawPoint";

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

    private totalPoints(drawing: Stroke[]): number {
        let totalPoints: number = 0;
        drawing.forEach((line: Stroke) => {
            totalPoints += line.StylusPoints.length;
        });
        return totalPoints;
    }

    private async classic(drawing: Stroke[], level: number): Promise<void> {
        let totalPoints: number = this.totalPoints(drawing);
        for(let stroke of drawing) {
            if(this.roomId) {
                this.io.in(this.roomId).emit("new_stroke", JSON.stringify(stroke));
            } else {
                this.io.emit("new_stroke", JSON.stringify(stroke));
            }
            for(let stylusPoint of stroke.StylusPoints) {
                await VirtualPlayer.delay(this.timePerRound * 1000 / totalPoints);
                if(this.roomId) {
                    this.io.in(this.roomId).emit("new_point", JSON.stringify(stylusPoint));
                    console.log(stylusPoint);
                    console.log(this.getDistance(stylusPoint, {X: 180, Y: 120}));
                    console.log("***");
                } else  {
                    this.io.emit("new_point", JSON.stringify(stylusPoint));
                }
            }
        }
    }

    // Source : https://javascript.info/task/shuffle
    private async random(drawing: Stroke[], level: number): Promise<void> {
        for(let i: number = drawing.length - 1; i > 0; i--) {
            let j: number = Math.floor(Math.random() * (i + 1));
            [drawing[i], drawing[j]] = [drawing[j], drawing[i]];
        }

        await this.classic(drawing, level);
    }

    private async panoramic(drawing: Stroke[], level: number): Promise<void> {
        // Gauche a droite pour le moment
        drawing.sort((a: Stroke, b: Stroke) => {
            let aMin: number = a.StylusPoints.reduce((min, stylusPoint) => stylusPoint.X < min ? stylusPoint.X : min, a.StylusPoints[0].X);
            let bMin: number = b.StylusPoints.reduce((min, stylusPoint) => stylusPoint.X < min ? stylusPoint.X : min, b.StylusPoints[0].X);
            return aMin - bMin;
        });

        await this.classic(drawing, level);
    }

    private async centered(drawing: Stroke[], level: number): Promise<void> {
        // Point central : (180, 120)
        const center: StylusPoint = {
            X: 180,
            Y: 120
        }

        drawing.sort((a: Stroke, b: Stroke) => {
            let aMin: number = this.getDistance(a.StylusPoints[0], center);
            for(let i: number = 1; i < a.StylusPoints.length; i++) {
                if(this.getDistance(a.StylusPoints[i], center) < aMin) {
                    aMin = this.getDistance(a.StylusPoints[i], center);

                }
            }
            // console.log(aMin);
            let bMin: number = this.getDistance(b.StylusPoints[0], center);
            for(let i: number = 1; i < b.StylusPoints.length; i++) {
                if(this.getDistance(b.StylusPoints[i], center) < bMin) {
                    bMin = this.getDistance(b.StylusPoints[i], center);

                }
            }
            // console.log(bMin);
            return aMin - bMin;
        });

        await this.classic(drawing, level);
    }

    private getDistance(a: StylusPoint, b: StylusPoint): number {
        return Math.hypot(a.X - b.X, a.Y - b.Y);
    }

    private static async delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}