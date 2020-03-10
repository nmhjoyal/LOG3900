export interface RGB {
    r: number
    g: number
    b: number
}

export interface Position {
    x: number
    y: number
}

export default interface DrawPoint {
    color: RGB
    pos: Position
    width: number
}