import SignIn from "../models/signIn";
import PrivateProfile from "../models/privateProfile";
import { Room, CreateRoom, Invitation } from "../models/room";
import { Feedback, SignInFeedback, SignInStatus, SignOutStatus, UpdateProfileStatus, JoinRoomFeedback, StartMatchFeedback, CreateMatchFeedback } from "../models/feedback";
import { profileDB } from "../services/Database/profileDB";
import { roomDB } from "../services/Database/roomDB";
import Admin from "../models/admin";
import { Message, ClientMessage } from "../models/message";
import PublicProfile from "../models/publicProfile";
import ChatHandler from "./chatHandler";
import MatchHandler from "./matchHandler";
import RandomMatchIdGenerator from "./IdGenerator/idGenerator";
import { CreateMatch, StartMatch } from "../models/match";

class ServerHandler {
    public users: Map<string, PrivateProfile>;
    public chatHandler: ChatHandler;
    public matchHandler: MatchHandler;

    public constructor () {
        this.users = new Map<string, PrivateProfile>();
        this.chatHandler = new ChatHandler();
        this.matchHandler = new MatchHandler();
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

    public signOut(io: SocketIO.Server, socket: SocketIO.Socket): Feedback {
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        let status: boolean = false;
        let log_message: SignOutStatus = SignOutStatus.Error;
        if(user) {

            // TEMPORARY
            this.matchHandler.leaveFreeDrawTestRoom(io, socket);

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

    /**
     * 
     * Other functions.
     * 
     */   

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
                this.chatHandler.deleteChatRoom(io, socket, roomId, this.getUser(socket.id));
            } else {
                const message: Message = Admin.createAdminMessage(user.username + " is disconnected.", roomId);
                this.chatHandler.privateRooms.find(room => room.id == roomId)?.messages.push(message);
                socket.to(roomId).emit("new_message", JSON.stringify(message));
                socket.leave(roomId);
            }
        }
    }

    public async updateProfile(io: SocketIO.Server, socket: SocketIO.Socket, updatedProfile: PrivateProfile): Promise<Feedback> {
        const user: PrivateProfile | undefined = this.users.get(socket.id);
        let status: boolean = false;
        let log_message: UpdateProfileStatus;

        if (user) {
            if (user.username == updatedProfile.username) {
                try {
                    // Updating rooms_joined array
                    updatedProfile.rooms_joined = (user as PrivateProfile).rooms_joined;
                    await profileDB.updateProfile(updatedProfile);

                    if (user.avatar != updatedProfile.avatar) {
                        await this.updateAvatarInRooms(io, user.username, updatedProfile.avatar);
                    }

                    this.users.set(socket.id, updatedProfile);
                    
                    status = true;
                    log_message = UpdateProfileStatus.Update;
                } catch {
                    log_message = UpdateProfileStatus.UnexpectedError;
                }
            } else {
                log_message = UpdateProfileStatus.InvalidUsername;
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

    private async updateAvatarInRooms(io: SocketIO.Server, username: string, newAvatar: string): Promise<void> {
        
        const updatedPublicProfile: PublicProfile = {
            username: username,
            avatar: newAvatar
        };
    
        // For each room retrieved from db (all the rooms that the user has been in or is currently in)
        // Update and notify that his avatar has changed.
        (await roomDB.getRoomsByUser(username)).forEach(async (roomId: string) => {
            await roomDB.mapAvatar(updatedPublicProfile, roomId);
            this.chatHandler.notifyAvatarUpdate(io, updatedPublicProfile, roomId);
        });
        
        // Update avatar map in the private rooms and notify the rooms
        for (let room of this.chatHandler.privateRooms) {
            if (room.avatars.has(updatedPublicProfile.username)) {
                room.avatars.set(username, newAvatar);
                this.chatHandler.notifyAvatarUpdate(io, updatedPublicProfile, room.id);
            }
        }
    }
    
    /**
     * 
     * ChatHandler and MatchHandler function calls while passing the private profile of the user.
     * 
     */
    public async createChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, room: CreateRoom): Promise<Feedback> {
        return await this.chatHandler.createChatRoom(io, socket, room, this.getUser(socket.id));
    }

    public async deleteChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        return await this.chatHandler.deleteChatRoom(io, socket, roomId, this.getUser(socket.id));
    }

    public async invite(io: SocketIO.Server, socket: SocketIO.Socket, invitation: Invitation): Promise<Feedback> {
        return await this.chatHandler.invite(io, socket, invitation, this.users);
    }

    public async joinChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<JoinRoomFeedback> {
        return await this.chatHandler.joinChatRoom(io, socket, roomId, this.getUser(socket.id));
    }

    public async leaveChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        return await this.chatHandler.leaveChatRoom(io, socket, roomId, this.getUser(socket.id));
    }

    public sendMessage(io: SocketIO.Server, socket: SocketIO.Socket, message: ClientMessage): void { 
        const user: PrivateProfile | undefined = this.getUser(socket.id);
        if (user) {
            if (message.roomId.startsWith(RandomMatchIdGenerator.prefix)) {
                this.matchHandler.sendMessage(io, socket, message, user);
            } else {
                // Send the message
                this.chatHandler.sendMessage(io, message, user);
            }
        } else {
            console.log("User not signed in");
        }
    }

    public async createMatch(io: SocketIO.Server, socket: SocketIO.Socket, createMatch: CreateMatch): Promise<CreateMatchFeedback> {
        return await this.matchHandler.createMatch(io, socket, createMatch, this.getUser(socket.id));
    }

    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, matchId: string): Promise<JoinRoomFeedback> {
        return await this.matchHandler.joinMatch(io, socket, matchId, this.getUser(socket.id));
    }

    public async leaveMatch(io: SocketIO.Server, socket: SocketIO.Socket): Promise<Feedback> {
        return await this.matchHandler.leaveMatch(io, socket, this.getUser(socket.id));
    }

    public addVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket): Feedback {
        return this.matchHandler.addVirtualPlayer(io, socket, this.getUser(socket.id));
    }

    public removeVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket): Feedback {
        return this.matchHandler.removeVirtualPlayer(io, socket, this.getUser(socket.id));
    }

    public startMatch(io: SocketIO.Server, socket: SocketIO.Socket, startMatch: StartMatch): StartMatchFeedback {
        return this.matchHandler.startMatch(io, socket, startMatch, this.getUser(socket.id));
    }

    public startTurn(io: SocketIO.Server, socket: SocketIO.Socket, word: string): void {
        return this.matchHandler.startTurn(io, socket, word, this.getUser(socket.id));
    }
}

export var serverHandler: ServerHandler = new ServerHandler();

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