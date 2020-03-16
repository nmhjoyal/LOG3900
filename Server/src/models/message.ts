export interface Message {
    username : string
    content : string
    date : number
    roomId : string
}

export interface ClientMessage {
    content : string
    roomId : string
}