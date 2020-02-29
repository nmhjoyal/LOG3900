// Il faut aussi envoyer la room lorsque le client recoit le message
export default interface Message {
    username : string
    content : string
    date : number
    roomId : string
}

export default interface ClientMessage {
    content : string
    roomId : string
}