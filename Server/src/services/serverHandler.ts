import User from "../models/user";
import ChatRoom from "./chatRoom";

export default class ServerHandler {
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
    public signInUser(socketId: string, user: User): boolean {

        let userSignedIn: boolean = false;

        if (!this.isConnected(user.username)) {
            this.users.set(socketId, user);
            userSignedIn = true;
        }

        return userSignedIn;
    }

    public signOutUser(socketId: string): void {
        this.users.delete(socketId);
    }

    public getUsers(): Map<string, User> {
        return this.users;
    }

    public getUser(socketId: string): User | undefined {
        return this.users.get(socketId);
    }

    private isConnected(username: string): boolean {
        let userIsConnected: boolean = false;

        this.users.forEach((value: User) => {
            if (value.username === username) {
                userIsConnected = true;
            }
        });

        return userIsConnected;
    }

    public joinRoom(roomName: string): void {
        // eventually
    }
}