export interface Color {
    r: number
    g: number
    b: number
}

export default interface Point {
    x: number
    y: number
}

export interface Trace {
    color: Color
    point: Point
    width: number
    tool: string
}

export interface Line {
    DrawingAttributes: DrawingAttributes
    StylusPoints: StylusPoint[]
}

export interface DrawingAttributes {
    Color: string,
    Width: number
}

export interface StylusPoint {
    X: number,
    Y: number
}