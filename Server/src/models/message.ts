import User from "./user";

export default interface Message {
    author : User
    content : string
    date : any
}