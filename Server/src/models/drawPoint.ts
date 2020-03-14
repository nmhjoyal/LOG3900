export interface RGB {
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
}