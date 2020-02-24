import Message from "../models/message";
import PublicProfile from "../models/publicProfile";

export default class ChatRoom {
    public name: string;
    private users: PublicProfile[];
    private messages: Message[];

    public constructor(name: string) {
        this.name = name;
        this.users = [];
        this.messages = [];
    }

    public addUser(user: PublicProfile): void {
        this.users.push(user);
    }

    public removeUser(user: PublicProfile): void {
        this.users.splice(this.users.indexOf(user), 1);
    }

    public getUsers(): PublicProfile[] {
        return this.users;
    }

    public addMessage(message: Message): void {
        this.messages.push(message);
    }

    public getUser(username: string): PublicProfile | undefined {
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