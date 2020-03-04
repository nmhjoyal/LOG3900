import { Room, CreateRoom, Invitation } from "../models/room";
import PrivateProfile from "../models/privateProfile";
import { Feedback, CreateRoomStatus, DeleteRoomStatus, JoinRoomStatus, LeaveRoomStatus, JoinRoomFeedback, InviteStatus } from "../models/feedback";
import { roomDB } from "./Database/roomDB";
import Admin from "../models/admin";
import { Message, ClientMessage } from "../models/message";
import { profileDB } from "./Database/profileDB";
import { serverHandler } from "./serverHandler";
import ChatFilter from "./Filter/chatFilter";
import PublicProfile from "../models/publicProfile";
import AvatarUpdate from "../models/avatarUpdate";


export default class ChatHandler {

    public privateRooms: Room[];

    public constructor() {
        this.privateRooms = [];
    }

    public async createChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, room: CreateRoom): Promise<Feedback> {
        let status: boolean = false;
        let log_message: CreateRoomStatus;
        const roomId: string = room.id;
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        
        if (user) {
            const roomIds: string[] = await roomDB.getRooms();
            if (!(roomIds.includes(roomId) || this.privateRooms.find(room => room.id == roomId))) {
                if (room.isPrivate) { // private 
                    const message: Message = this.connectSocketToRoom(io, socket, user, roomId);
                    const newRoom: Room = this.createRoomObject(roomId, user.username, user.avatar, message);
                    this.privateRooms.push(newRoom);
                    status = true;
                    log_message = CreateRoomStatus.Create;
                } else { // public room
                    try {
                        const message: Message = this.connectSocketToRoom(io, socket, user, roomId);
                        const newRoom: Room = this.createRoomObject(roomId, user.username, user.avatar, message);
                        await roomDB.createRoom(newRoom);
                        await profileDB.joinRoom(user.username, roomId);
                        status = true;
                        log_message = CreateRoomStatus.Create;
                    } catch {
                        // Room already exists
                        log_message = CreateRoomStatus.Error;
                    }
                }
            } else {
                // Room id already taken
                log_message = CreateRoomStatus.AlreadyCreated
            }
        } else {
            log_message = CreateRoomStatus.InvalidUser;
        }
        
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }

        return feedback;
    }

    public async deleteChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        const privateRoom: Room | undefined = this.privateRooms.find(room => room.id == roomId);
        let status: boolean = false;
        let log_message: DeleteRoomStatus;

        if (roomId != "General") {
            if (user) {
                let socketIds: string[] = this.getSocketIds(io, roomId);
                if (room) {
                    if (socketIds.length == 0) {
                        await roomDB.deleteRoom(roomId);
                        await profileDB.deleteRoom(roomId);
                        status = true;
                        log_message = DeleteRoomStatus.Delete;
                    } else if (socketIds.length == 1 && socketIds[0] == socket.id) {
                        user.rooms_joined.splice(user.rooms_joined.indexOf(roomId), 1);
                        socket.leave(roomId);
                        await roomDB.deleteRoom(roomId);
                        await profileDB.deleteRoom(roomId);
                        status = true;
                        log_message = DeleteRoomStatus.LeaveAndDelete;
                    } else {
                        log_message = DeleteRoomStatus.NotEmpty;
                    }
                } else if (privateRoom) {
                    if (socketIds.length == 0) {
                        log_message = DeleteRoomStatus.Error;
                        console.log("nonono");
                        // should never happen because if we leave and we are the last person the room is deleted.
                    } else if (socketIds.length == 1 && socketIds[0] == socket.id) {
                        socket.leave(roomId);
                        this.privateRooms.splice(this.privateRooms.indexOf(privateRoom), 1);
                        status = true;
                        log_message = DeleteRoomStatus.LeaveAndDelete;
                    } else {
                        log_message = DeleteRoomStatus.NotEmpty;
                    }
                } else {
                    log_message = DeleteRoomStatus.InvalidRoom;
                }
            } else {
                log_message = DeleteRoomStatus.InvalidUser;
            }
        } else {
            log_message = DeleteRoomStatus.DeleteGeneral;
        }

        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        
        return feedback;
    }

    public async joinChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<JoinRoomFeedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let privateRoom: Room | undefined = this.privateRooms.find(room => room.id == roomId);
        let room_joined: Room | null = null;
        let status: boolean = false;
        let log_message: JoinRoomStatus;
        let isPrivate: boolean | null = null;

        if(user) {
            if (room) {
                if(user.rooms_joined.includes(room.id)) {
                    log_message = JoinRoomStatus.AlreadyJoined;
                } else {
                    await profileDB.joinRoom(user.username, room.id);
                    user.rooms_joined.push(room.id);
                    const publicProfile : PublicProfile = {
                        username : user.username,
                        avatar : user.avatar
                    };
                    await roomDB.mapAvatar(publicProfile, room.id);
                    this.notifyAvatarUpdate(io, publicProfile, room.id);
                    await roomDB.addMessage(this.connectSocketToRoom(io, socket, user, room.id));
                    room_joined = room;
                    status = true;
                    log_message = JoinRoomStatus.Join;
                    isPrivate = false;
                }
            } else if (privateRoom) {
                privateRoom.avatars.set(user.username, user.avatar);
                privateRoom.messages.push(this.connectSocketToRoom(io, socket, user, privateRoom.id));
                room_joined = privateRoom;
                status = true;
                log_message = JoinRoomStatus.Join;
                isPrivate = true;
            } else {
                log_message = JoinRoomStatus.InvalidRoom;
            }
        } else {
            log_message = JoinRoomStatus.InvalidUser;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        const joinRoomFeedback: JoinRoomFeedback = {
            feedback: feedback,
            room_joined: room_joined,
            isPrivate: isPrivate
        }
        
        return joinRoomFeedback;
    }

    public async leaveChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string): Promise<Feedback> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(roomId);
        let privateRoom: Room | undefined = this.privateRooms.find(room => room.id == roomId);
        let status: boolean = false;
        let log_message: LeaveRoomStatus;
        
        if(user) {
            if (room) {
                if(user.rooms_joined.includes(room.id)) {
                    if(roomId == "General") {
                        log_message = LeaveRoomStatus.General;
                    } else {
                        await roomDB.addMessage(this.disconnectSocketFromRoom(socket, user, room.id));
                        user.rooms_joined.splice(user.rooms_joined.indexOf(room.id), 1);
                        await profileDB.leaveRoom(user.username, room.id);
                        status = true;
                        log_message = LeaveRoomStatus.Leave;
                    }
                } else {
                    log_message = LeaveRoomStatus.NeverJoined;
                }
            } else if (privateRoom) {
                let socketIds: string[] = this.getSocketIds(io, roomId);
                if (socketIds.length == 1 && socketIds[0] == socket.id) {
                    this.deleteChatRoom(io, socket, roomId);
                    log_message = LeaveRoomStatus.LeaveAndDelete;
                } else {
                    privateRoom.messages.push(this.disconnectSocketFromRoom(socket, user, privateRoom.id));
                    log_message = LeaveRoomStatus.Leave;
                }
                status = true;
            } else {
                log_message = LeaveRoomStatus.InvalidRoom;
            }
        } else {
            log_message = LeaveRoomStatus.InvalidUser;
        }
        const leaveRoomFeedback: Feedback = {
            status: status,
            log_message: log_message
        }
        
        return leaveRoomFeedback;
    }

    public async sendMessage(io: SocketIO.Server, socket: SocketIO.Socket, clientMessage: ClientMessage): Promise<void> {
        const user: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        const room: Room | null = await roomDB.getRoom(clientMessage.roomId);
        let privateRoom: Room | undefined = this.privateRooms.find(room => room.id == clientMessage.roomId);
        
        if (user) {
            const message: Message = {
                username : user.username,
                content : ChatFilter.filter(clientMessage.content),
                date : Date.now(),
                roomId : clientMessage.roomId
            };
            if (room) {
                // message.author.username == user.username
                if(user.rooms_joined.includes(clientMessage.roomId)) {
                    await roomDB.addMessage(message);
                    io.in(message.roomId).emit("new_message", JSON.stringify(message));
                } else {
                    console.log(user.username + " trying to send a message in " + clientMessage.roomId + " that he did not join");
                }
            } else if (privateRoom) {
                privateRoom.messages.push(message);
                io.in(message.roomId).emit("new_message", JSON.stringify(message));
            } else {
                console.log(user?.username + " trying to send a message in " + clientMessage.roomId + " that does not exist");
            }
        } else {
            console.log("The user does not exist");
        }
    }

    public async invite(io: SocketIO.Server, socket: SocketIO.Socket, invitation: Invitation): Promise<Feedback> {
        const sender: PrivateProfile | undefined = serverHandler.users.get(socket.id);
        let receiver: PrivateProfile | undefined;
        let receiverSocketId: string | undefined;
        serverHandler.users.forEach((user: PrivateProfile, socketId: string) => {
            if(user.username == invitation.username) {
                receiver = user;
                receiverSocketId = socketId;
            }
        });
        const room: Room | null = await roomDB.getRoom(invitation.id);
        let privateRoom: Room | undefined = this.privateRooms.find(room => room.id == invitation.id);
        let status: boolean = false;
        let log_message: InviteStatus;
        if(sender) {
            if (receiver && receiverSocketId) {
                if(room || privateRoom) {
                    if (this.getSocketIds(io, invitation.id).includes(socket.id)) {
                        if(!this.getSocketIds(io, invitation.id).includes(receiverSocketId)) {
                            console.log(invitation);
                            invitation.username = sender.username;
                            io.to(receiverSocketId).emit("receive_invite", JSON.stringify(invitation));
                            status = true;
                            log_message = InviteStatus.Invite;
                        } else {
                            log_message = InviteStatus.ReceiverInRoom;
                        }
                    } else {
                        log_message = InviteStatus.SenderOutRoom;
                    }
                } else {
                    log_message = InviteStatus.InvalidRoom;
                }
            } else {
                log_message = InviteStatus.InvalidReceiver;
            }
        } else {
            log_message = InviteStatus.InvalidSender;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        }
        return feedback;
    }

    private createRoomObject(roomId: string, username: string, avatar: string, message: Message): Room {
        let avatars: Map<string, string> = new Map<string, string>();
        // Map avatar du createur de la room, car il join directement à la création.
        avatars.set(username, avatar);
        const room: Room = {
            id: roomId,
            messages: [message],
            avatars: avatars
        };

        return room;
    }

    private connectSocketToRoom(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile, roomId: string): Message {
        socket.join(roomId);
        const message: Message = Admin.createAdminMessage(user.username + " joined the room.", roomId);
        socket.to(roomId).emit("new_message", JSON.stringify(message));
        return message;
    }

    private disconnectSocketFromRoom(socket: SocketIO.Socket, user: PrivateProfile, roomId: string): Message {
        socket.leave(roomId);
        const message: Message = Admin.createAdminMessage( user.username + " left the room.", roomId);
        socket.to(roomId).emit("new_message", JSON.stringify(message));
        return message;
    }

    public getSocketIds(io: SocketIO.Server, roomId: string): string[] {
        let socketIds: string[] = [];
        try {
            for (let socketId in io.sockets.adapter.rooms[roomId].sockets) {
                socketIds.push(socketId);
            }
        } catch {}

        return socketIds;
    }

    public notifyAvatarUpdate(io: SocketIO.Server, publicProfile: PublicProfile, roomId: string) : void {
        const avatarUpdate: AvatarUpdate = {
            roomId: roomId,
            updatedProfile: publicProfile 
        };
        io.in(roomId).emit("avatar_updated", JSON.stringify(avatarUpdate));
    }
}