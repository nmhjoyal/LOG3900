import { Message }  from "./message";

export default interface Room {
    name: string
    messages: Message[]
    avatars: Map<string, string> // (key: users[i].username, value: users[i].avatar) 
}