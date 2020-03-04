import SignIn from "../models/signIn";
import PrivateProfile from "../models/privateProfile";
import { Room } from "../models/room";
import { Feedback, SignInFeedback, SignInStatus, SignOutStatus, UpdateProfileStatus } from "../models/feedback";
import { profileDB } from "../services/Database/profileDB";
import { roomDB } from "../services/Database/roomDB";
import Admin from "../models/admin";
import { Message } from "../models/message";
import AvatarUpdate from "../models/avatarUpdate";
import PublicProfile from "../models/publicProfile";
import ChatHandler from "./chatHandler";

class ServerHandler {
    public users: Map<string, PrivateProfile>;
    public chatHandler: ChatHandler;

    public constructor () {
        this.users = new Map<string, PrivateProfile>();
        this.chatHandler = new ChatHandler();
    }

    public async signIn(socket: SocketIO.Socket, signIn: SignIn): Promise<SignInFeedback> {
        const user: PrivateProfile | null = await profileDB.getPrivateProfile(signIn.username);
        let signed_in: boolean = false;
        let log_message: SignInStatus;
        let rooms_joined: Room[] = [];
        if(user) {
            if(signIn.password == user.password) {
                if(this.isConnected(signIn.username)) {
                    log_message = SignInStatus.AlreadyConnected
                } else {
                    this.users.set(socket.id, user);
                    signed_in = true;
                    log_message = SignInStatus.SignIn;
                    rooms_joined = await this.connectToJoinedRooms(socket, user);
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

    public async signOut(io: SocketIO.Server, socket: SocketIO.Socket): Promise<Feedback> {
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        let status: boolean = false;
        let log_message: SignOutStatus = SignOutStatus.Error;
        if(user) {
            this.diconnectFromJoinedRooms(io, socket, user);
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

    private async connectToJoinedRooms(socket: SocketIO.Socket, user: PrivateProfile): Promise<Room[]> {
        const rooms: Room[] = [];
        for(let room_joined of user.rooms_joined) {
            socket.join(room_joined);
            const message: Message = Admin.createAdminMessage(user.username + " is connected.", room_joined);
            socket.to(room_joined).emit("new_message", JSON.stringify(message));
            const room: Room | null = await roomDB.getRoom(room_joined);
            if(room) {
                rooms.push(room);
            } else {
                console.log("This room does not exist : " + room_joined);
            }
        }
        return rooms;
    }

    private diconnectFromJoinedRooms(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile): void {
        // Public rooms
        user.rooms_joined.forEach((room_joined: string) => {
            const message: Message = Admin.createAdminMessage(user.username + " is disconnected.", room_joined);
            socket.to(room_joined).emit("new_message", JSON.stringify(message));
            socket.leave(room_joined);
        });
        // Private rooms
        for (let roomId in socket.rooms) {
            let socketIds: string[] = this.chatHandler.getSocketIds(io, roomId);
            if (socketIds.length == 1 && socketIds[0] == socket.id) {
                this.chatHandler.deleteChatRoom(io, socket, roomId);
            } else {
                const message: Message = Admin.createAdminMessage(user.username + " is disconnected.", roomId);
                this.chatHandler.privateRooms.find(room => room.id == roomId)?.messages.push(message);
                socket.to(roomId).emit("new_message", JSON.stringify(message));
                socket.leave(roomId);
            }
        }
    }

    public async updateProfile(io: SocketIO.Server, socket: SocketIO.Socket, updatedProfile: PrivateProfile): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        let status: boolean = false;
        let log_message: UpdateProfileStatus;

        if(user) {
            try {
                // Making sure rooms_joined is not updated
                updatedProfile.rooms_joined = (user as PrivateProfile).rooms_joined;
                await profileDB.updateProfile(updatedProfile);
    
                // No errors, so update PrivateProfile 
                // and if necessary notify rooms that the user's avatar has changed.
                this.users.forEach(async (user: PrivateProfile, socketId: string) => {
                    if(user.username == updatedProfile.username) {
                        if (user.avatar != updatedProfile.avatar) {
                            // Notify all rooms joined by user that his avatar has changed.
                            const updatedPublicProfile: PublicProfile = {
                                username: user.username,
                                avatar: updatedProfile.avatar
                            };
                            // For each room retrieved from db
                            (await roomDB.getRoomsByUser(user.username)).forEach(async (roomId: string) => {
                                await roomDB.mapAvatar(updatedPublicProfile, roomId);
                                const avatarUpdate: AvatarUpdate = {
                                    roomId: roomId,
                                    updatedProfile: updatedPublicProfile 
                                };
                                io.in(roomId).emit("avatar_updated", JSON.stringify(avatarUpdate));
                            });
                        }
                        this.users.set(socketId, updatedProfile);
                    }
                });
                status = true;
                log_message = UpdateProfileStatus.Update;
            } catch {
                log_message = UpdateProfileStatus.UnexpectedError;
            }
        } else {
            log_message = UpdateProfileStatus.InvalidProfile;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        return feedback;
    }

    /*
    public getUsersOutsideRoom(roomId: string): PublicProfile[] {
        let usersOutsideRoom: PublicProfile[] = [];
        this.users.forEach((user: PrivateProfile) => {
            if (!user.rooms_joined.includes(roomId)) {
                const publicProfile: PublicProfile = {
                    username: user.username,
                    avatar: user.avatar
                }
                usersOutsideRoom.push(publicProfile)
            }
        });
        return usersOutsideRoom;
    }
    */
}

export var serverHandler: ServerHandler = new ServerHandler();