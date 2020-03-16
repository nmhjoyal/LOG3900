export interface Color {
    r: number
    g: number
    b: number
}

export interface Point {
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

export interface Game {
    word: string,
    drawing: Line[],
    clues: string[],
    level: Level,
    mode: Mode
}

export interface GamePreview {
    drawing: Line[],
    mode: Mode
}

export enum Level {
    Easy,
    Medium,
    Hard
}

export enum Mode {
    Classic,
    Random,
    Panoramic,
    Centered
}