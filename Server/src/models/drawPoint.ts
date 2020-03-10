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
    rgb: Color
    pos: Position
    width: number
}