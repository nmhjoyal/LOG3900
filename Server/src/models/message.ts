// Il faut aussi envoyer la room lorsque le client recoit le message
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