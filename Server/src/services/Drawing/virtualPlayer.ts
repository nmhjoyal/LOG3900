import { Game, Stroke, Mode, GamePreview, Level, StylusPoint } from "../../models/drawPoint";

// 1 virtual player per game / per preview
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
        if(this.roomId) {
            this.io.in(this.roomId).emit("clear");
        } else {
            this.io.emit("clear");
        }
        switch(game.mode) {
            case Mode.Classic :
                await this.classic(game.drawing, game.level);
                break;
            case Mode.Random :
                await this.random(game.drawing, game.level);
                break;
            case Mode.Panoramic :
                await this.panoramic(game.drawing, game.level, game.option);
                break;
            case Mode.Centered :
                await this.centered(game.drawing, game.level, game.option);
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
        if(this.roomId) {
            this.io.in(this.roomId).emit("clear");
        } else {
            this.io.emit("clear");
        }
        switch(gamePreview.mode) {
            case Mode.Classic :
                await this.classic(gamePreview.drawing, Level.Hard);
                break;
            case Mode.Random :
                await this.random(gamePreview.drawing, Level.Hard);
                break;
            case Mode.Panoramic :
                await this.panoramic(gamePreview.drawing, Level.Hard, gamePreview.option);
                break;
            case Mode.Centered :
                await this.centered(gamePreview.drawing, Level.Hard, gamePreview.option);
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

    private async panoramic(drawing: Stroke[], level: number, option: number): Promise<void> {
        switch(option) {
            // Left to right
            case 0:
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMin: number = a.StylusPoints.reduce((min, stylusPoint) => stylusPoint.X < min ? stylusPoint.X : min, a.StylusPoints[0].X);
                    let bMin: number = b.StylusPoints.reduce((min, stylusPoint) => stylusPoint.X < min ? stylusPoint.X : min, b.StylusPoints[0].X);
                    return aMin - bMin;
                });
                break;
            // Right to left
            case 1:
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMax: number = a.StylusPoints.reduce((max, stylusPoint) => stylusPoint.X > max ? stylusPoint.X : max, a.StylusPoints[0].X);
                    let bMax: number = b.StylusPoints.reduce((max, stylusPoint) => stylusPoint.X > max ? stylusPoint.X : max, b.StylusPoints[0].X);
                    return bMax - aMax;
                });
                break;
            // Up to bottom
            case 2:
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMin: number = a.StylusPoints.reduce((min, stylusPoint) => stylusPoint.Y < min ? stylusPoint.Y : min, a.StylusPoints[0].Y);
                    let bMin: number = b.StylusPoints.reduce((min, stylusPoint) => stylusPoint.Y < min ? stylusPoint.Y : min, b.StylusPoints[0].Y);
                    return aMin - bMin;
                });
                break;
            // Bottom to up
            case 3:
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMax: number = a.StylusPoints.reduce((max, stylusPoint) => stylusPoint.Y > max ? stylusPoint.Y : max, a.StylusPoints[0].Y);
                    let bMax: number = b.StylusPoints.reduce((max, stylusPoint) => stylusPoint.Y > max ? stylusPoint.Y : max, b.StylusPoints[0].Y);
                    return bMax - aMax;
                });
                break;
        }

        await this.classic(drawing, level);
    }

    private async centered(drawing: Stroke[], level: number, option: number): Promise<void> {
        // Point central : (300, 300)
        const center: StylusPoint = {
            X: 300,
            Y: 300
        }
        switch(option) {
            case 0 :
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMin: number = this.getDistance(a.StylusPoints[0], center);
                    for(let i: number = 1; i < a.StylusPoints.length; i++) {
                        if(this.getDistance(a.StylusPoints[i], center) < aMin) {
                            aMin = this.getDistance(a.StylusPoints[i], center);
        
                        }
                    }
                    let bMin: number = this.getDistance(b.StylusPoints[0], center);
                    for(let i: number = 1; i < b.StylusPoints.length; i++) {
                        if(this.getDistance(b.StylusPoints[i], center) < bMin) {
                            bMin = this.getDistance(b.StylusPoints[i], center);
        
                        }
                    }
                    return aMin - bMin;
                });
                break;
            case 1 :
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMax: number = this.getDistance(a.StylusPoints[0], center);
                    for(let i: number = 1; i < a.StylusPoints.length; i++) {
                        if(this.getDistance(a.StylusPoints[i], center) > aMax) {
                            aMax = this.getDistance(a.StylusPoints[i], center);
        
                        }
                    }
                    let bMax: number = this.getDistance(b.StylusPoints[0], center);
                    for(let i: number = 1; i < b.StylusPoints.length; i++) {
                        if(this.getDistance(b.StylusPoints[i], center) > bMax) {
                            bMax = this.getDistance(b.StylusPoints[i], center);
        
                        }
                    }
                    return bMax - aMax;
                });
                break;
        }

        await this.classic(drawing, level);
    }

    private getDistance(a: StylusPoint, b: StylusPoint): number {
        return Math.hypot(a.X - b.X, a.Y - b.Y);
    }

    private static async delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }
}