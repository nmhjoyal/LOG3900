import { Stroke, StylusPoint, Mode } from "../../models/drawPoint";

export class Utils {

    public static sort(drawing: Stroke[], mode: Mode, option: number): void {
        let top: number = 0;
        drawing.forEach((stroke: Stroke) => {
            stroke.DrawingAttributes.Top = top++;
        });
        switch(mode) {
            case Mode.Random:
                Utils.random(drawing);
                break;
            case Mode.Panoramic:
                Utils.panoramic(drawing, option);
                break;
            case Mode.Centered:
                Utils.centered(drawing, option);
                break;
        }
    }

    private static random(drawing: Stroke[]): void {
        for(let i: number = drawing.length - 1; i > 0; i--) {
            let j: number = Math.floor(Math.random() * (i + 1));
            [drawing[i], drawing[j]] = [drawing[j], drawing[i]];
        }
    }

    private static panoramic(drawing: Stroke[], option: number): void {
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
    }

    private static centered(drawing: Stroke[], option: number): void {
        // Point central : (300, 300)
        const center: StylusPoint = {
            X: 300,
            Y: 250
        }
        switch(option) {
            case 0 :
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMin: number = Utils.getDistance(a.StylusPoints[0], center);
                    for(let i: number = 1; i < a.StylusPoints.length; i++) {
                        if(Utils.getDistance(a.StylusPoints[i], center) < aMin) {
                            aMin = Utils.getDistance(a.StylusPoints[i], center);
        
                        }
                    }
                    let bMin: number = Utils.getDistance(b.StylusPoints[0], center);
                    for(let i: number = 1; i < b.StylusPoints.length; i++) {
                        if(Utils.getDistance(b.StylusPoints[i], center) < bMin) {
                            bMin = Utils.getDistance(b.StylusPoints[i], center);
        
                        }
                    }
                    return aMin - bMin;
                });
                break;
            case 1 :
                drawing.sort((a: Stroke, b: Stroke) => {
                    let aMax: number = Utils.getDistance(a.StylusPoints[0], center);
                    for(let i: number = 1; i < a.StylusPoints.length; i++) {
                        if(Utils.getDistance(a.StylusPoints[i], center) > aMax) {
                            aMax = Utils.getDistance(a.StylusPoints[i], center);
        
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
    }

    public static totalPoints(drawing: Stroke[]): number {
        let totalPoints: number = 0;
        drawing.forEach((line: Stroke) => {
            totalPoints += line.StylusPoints.length;
        });
        return totalPoints;
    }

    public static uniform(drawing: Stroke[], uniformedPoints: number): Stroke[] {
        const totalPoints: number = this.totalPoints(drawing);
        if(uniformedPoints >= totalPoints /*|| uniformedPoints < drawing.length*/) {
            console.log("Not uniformed");
            return drawing;
        }
        const step: number = this.totalPoints(drawing) / uniformedPoints;
        const uniformedDrawing: Stroke[] = [];
        if(drawing.every(stroke => stroke.StylusPoints.length == 0)) {
            for(let i: number = 0; i < step; i++) {
                uniformedDrawing.push(drawing[Math.min(drawing.length - 1, Math.round(i * step))]);
            }
        } else {
            for(let i: number = 0; i < drawing.length; i++) {
                const stroke: Stroke = {
                    DrawingAttributes: drawing[i].DrawingAttributes,
                    StylusPoints: []
                }
                uniformedDrawing.push(stroke);
                for(let j: number = 0; j < (drawing[i].StylusPoints.length) / step; j++) {
                    uniformedDrawing[i].StylusPoints.push(drawing[i].StylusPoints[Math.min(drawing[i].StylusPoints.length - 1, Math.round(j * step))]);
                }
            }
        }
        return uniformedDrawing;
    }

    private static getDistance(a: StylusPoint, b: StylusPoint): number {
        return Math.hypot(a.X - b.X, a.Y - b.Y);
    }
}