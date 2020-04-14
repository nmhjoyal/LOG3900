import { Message }  from "./message";

export interface Room {
    id: string
    messages: Message[]
    avatars: Map<string, string> // (key: users[i].username, value: users[i].avatar) 
}

export interface CreateRoom {
    id: string
    isPrivate: boolean
}

export interface Invitation {
    id: string,
    username: string
}