import User from "../models/user";
import Message from "../models/message";

export default class ChatRoom {
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

    public toString() : string {
        let chatRoom: string = "";
        chatRoom += "\nName : " + this.name + "\nUsers : ";
        for(let user of this.users) {
            chatRoom += " " + user.username;
        }
        return chatRoom;
    }

    public contains(username: string): boolean {
        this.users.forEach((user) => {
            if (username == user.username) {
                return true;
            }
        });
        return false;
    }
}