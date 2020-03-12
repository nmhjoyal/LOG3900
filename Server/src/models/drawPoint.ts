export interface Color {
    r: number
    g: number
    b: number
}

export interface Position {
    x: number
    y: number
}

export default interface DrawPoint {
    color: Color
    pos: Position
    width: number
}