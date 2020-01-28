import { User } from "../models/user";
import { Message } from "../models/message";

export class Room {
    private name: string;
    private users: User[];
    private messages: Message[];

    public Room(name: string) {
        this.name = name;
        this.users = [];
        this.messages = [];
    }

    public addUser(user : User) : void {
        this.users.push(user);
    }

    public removeUser(user : User) : void {
        this.users.splice(this.users.indexOf(user), 1);
    }

    public getUsers() : User[] {
        return this.users;
    }

    public addMessage(message : Message) : void {
        this.messages.push(message);
    }

    public getMessages() : Message[] {
        return this.messages;
    }
}