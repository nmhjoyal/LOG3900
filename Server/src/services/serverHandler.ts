import User from "../models/user";
import ChatRoom from "./chatRoom";

export class ServerHandler {
    public name: string;
    private users: Map<string, User>;
    // TEMPORARY : eventually array of rooms
    private chatRoom: ChatRoom;

    public constructor(name: string) {
        this.name = name;
        this.users = new Map();
        // TEMPORARY
        this.chatRoom = new ChatRoom("TEMPORARY_NAME");
    }

    /**
     * Returns true if user is signed in
     * @param user user we wish to add
     */
    public signIn(socketId: string, user: User): boolean {

        let canSignIn: boolean = false;

        if (!this.isConnected(user)) {
            this.users.set(socketId, user);
            canSignIn = true;
        }

        return canSignIn;
    }

    public signOut(socketId: string): boolean {
        return this.users.delete(socketId);
    }

    public getUsers(): Map<string, User> {
        return this.users;
    }

    public getUser(socketId: string): User | undefined {
        return this.users.get(socketId);
    }

    private isConnected(user: User): boolean {
        return this.getUser(user.username) !== undefined;
    }

    public joinRoom(): void {

    }
}