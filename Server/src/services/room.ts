import { User } from "../models/user";
import { Message } from "../models/message";

export class Room {
    public name: string;
    private users: User[];
    private messages: Message[];

    public constructor(name: string) {
        this.name = name;
        this.users = [{ username: "Hubert", password: "123" }];
        this.messages = [];
    }

    public addUser(user: User): void {
        this.users.push(user);
    }

    public removeUser(user: User): void {
        this.users.splice(this.users.indexOf(user), 1);
    }

    public getUsers(): User[] {
        return this.users;
    }

    public addMessage(message: Message): void {
        this.messages.push(message);
    }

    public getUser(username: string): User | undefined {
        return this.users.find( user => user.username === username);
    }

    public getMessages() : Message[] {
        return this.messages;
    }
}