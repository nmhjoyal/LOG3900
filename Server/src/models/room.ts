import Message from "./message";

export default interface Room {
    name: string
    messages: Message[]
}