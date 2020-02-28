import PublicProfile from "./publicProfile";

// Il faut aussi envoyer la room lorsque le client recoit le message
export default interface Message {
    author : PublicProfile
    content : string
    date : number
    roomId : string
}