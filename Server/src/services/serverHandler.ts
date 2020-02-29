import SignIn from "../models/signIn";
import PrivateProfile from "../models/privateProfile";
import Room from "../models/room";
import { Feedback, SignInFeedback, SignInStatus, SignOutStatus } from "../models/feedback";
import { profileDB } from "../services/Database/profileDB";
import { roomDB } from "../services/Database/roomDB";
import Admin from "../models/admin";
import { Message } from "../models/message";

class ServerHandler {
    public users: Map<string, PrivateProfile>;

    public constructor () {
        this.users = new Map();
    }

    public async signIn(socket: SocketIO.Socket, signIn: SignIn): Promise<SignInFeedback> {
        const user: PrivateProfile | null = await profileDB.getPrivateProfile(signIn.username);
        let signed_in: boolean = false;
        let log_message: SignInStatus;
        let rooms_joined: string[] = [];
        if(user) {
            if(signIn.password == user.password) {
                if(this.isConnected(signIn.username)) {
                    log_message = SignInStatus.AlreadyConnected
                } else {
                    this.users.set(socket.id, user);
                    signed_in = true;
                    log_message = SignInStatus.SignIn;
                    rooms_joined = user.rooms_joined;
                    await this.connectToJoinedRooms(socket, user);
                }
            } else {
                log_message = SignInStatus.InvalidPassword;
            }
        } else {
            log_message = SignInStatus.InvalidUsername;
        }
        const feedback: Feedback = {
            status: signed_in,
            log_message: log_message
        }
        const signInFeedback: SignInFeedback = {
            feedback: feedback,
            rooms_joined: rooms_joined
        }
        return signInFeedback;
    }

    public async signOut(socket: SocketIO.Socket): Promise<Feedback> {
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        let status: boolean = false;
        let log_message: SignOutStatus = SignOutStatus.Error;
        if(user) {
            this.diconnectFromJoinedRooms(socket, user);
            this.users.delete(socket.id);
            status = true;
            log_message = SignOutStatus.SignedOut;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        return feedback;
    }

    public getUser(socketId: string): PrivateProfile | undefined {
        return this.users.get(socketId);
    }

    private isConnected(username: string): boolean {
        let isConnected: boolean = false;

        this.users.forEach((user: PrivateProfile) => {
            if (user.username === username) {
                isConnected = true;
            }
        });

        return isConnected;
    }

    private async connectToJoinedRooms(socket: SocketIO.Socket, user: PrivateProfile): Promise<void> {
        user.rooms_joined.forEach(async (room_joined: string) => {
            socket.join(room_joined);
            const message: Message = Admin.createAdminMessage(user.username + " is connected.", room_joined);
            socket.to(room_joined).emit("new_message", JSON.stringify(message));
            const room: Room | null = await roomDB.getRoom(room_joined);
            if(room) {
                socket.emit("load_history", JSON.stringify({
                    name: room.name,
                    messages: room.messages
                }));
            } else {
                console.log("This room does not exist : " + room_joined);
            }
        });
    }

    private diconnectFromJoinedRooms(socket: SocketIO.Socket, user: PrivateProfile): void {
        user.rooms_joined.forEach((room_joined: string) => {
            const message: Message = Admin.createAdminMessage(user.username + " is disconnected.", room_joined);
            socket.to(room_joined).emit("new_message", JSON.stringify(message));
            socket.leave(room_joined);
        });
    }

    // Pour deleteChatRoom : Room exists? -> Empty? -> Delete

    public updateUser(updatedUser: PrivateProfile): void {
        this.users.forEach((user: PrivateProfile, socketId: string) => {
            if(user.username == updatedUser.username) {
                this.users.set(socketId, updatedUser);
            }
        });
        // emit updated avatar
    }
}

export var serverHandler: ServerHandler = new ServerHandler();
// export var users: Map<string, PrivateProfile> = serverHandler.users;