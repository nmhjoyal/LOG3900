import PublicProfile from "./publicProfile";

export default interface Message {
    author : PublicProfile
    content : string
    date : number
}