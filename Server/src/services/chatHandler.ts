import { Room, CreateRoom, Invitation } from "../models/room";
import PrivateProfile from "../models/privateProfile";
import { Feedback, CreateRoomStatus, DeleteRoomStatus, JoinRoomStatus, LeaveRoomStatus, JoinRoomFeedback, InviteStatus } from "../models/feedback";
import { roomDB } from "./Database/roomDB";
import Admin from "../models/admin";
import { Message, ClientMessage } from "../models/message";
import { profileDB } from "./Database/profileDB";
import ChatFilter from "./Filter/chatFilter";
import PublicProfile from "../models/publicProfile";
import AvatarUpdate from "../models/avatarUpdate";


export default class ChatHandler {

    public privateRooms: Room[];

    public constructor() {
        this.privateRooms = [];
    }

    public async createChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, room: CreateRoom, user: PrivateProfile | undefined): Promise<Feedback> {
        let status: boolean = false;
        let log_message: CreateRoomStatus;
        const roomId: string = room.id;
        if (user) {
            if (!(this.findPrivateRoom(roomId) || (await roomDB.getRooms()).includes(roomId))) { // verifiy unicity 
                if (room.isPrivate) { // private 
                    const message: Message = this.connectSocketToRoom(socket, user.username, roomId);
                    const newRoom: Room = this.createRoomObject(roomId, user.username, user.avatar, message);
                    this.privateRooms.push(newRoom);
                    status = true;
                    log_message = CreateRoomStatus.Create;
                } else { // public room
                    try {
                        const message: Message = this.connectSocketToRoom(socket, user.username, roomId);
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
        console.log(feedback.log_message);
        return feedback;
    }

    public async deleteChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string, user: PrivateProfile | undefined): Promise<Feedback> {
        const privateRoom: Room | undefined = this.findPrivateRoom(roomId);
        let status: boolean = false;
        let log_message: DeleteRoomStatus;

        if (roomId != "General") {
            if (user) {
                let socketIds: string[] = this.getSocketIds(io, roomId);
                if (privateRoom) {
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
                } else if (await roomDB.getRoom(roomId)) {
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

    public async joinChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string, user: PrivateProfile | undefined): Promise<JoinRoomFeedback> {
        
        console.log(roomId);
        let privateRoom: Room | undefined = this.findPrivateRoom(roomId);
        let room_joined: Room | null = null;
        let status: boolean = false;
        let log_message: JoinRoomStatus;
        let isPrivate: boolean | null = null;

        if(user) {
            const publicProfile : PublicProfile = {
                username : user.username,
                avatar : user.avatar
            };
            if (privateRoom) {
                privateRoom.avatars.set(user.username, user.avatar);
                privateRoom.messages.push(this.connectSocketToRoom(socket, user.username, privateRoom.id));
                this.notifyAvatarUpdate(io, publicProfile, privateRoom.id);
                room_joined = privateRoom;
                status = true;
                log_message = JoinRoomStatus.Join;
                isPrivate = true;
            } else {
                const room: Room | null = await roomDB.getRoom(roomId);
                if (room) {
                    if(!user.rooms_joined.includes(room.id)) {
                        await profileDB.joinRoom(user.username, room.id);
                        user.rooms_joined.push(room.id);
                        await roomDB.mapAvatar(publicProfile, room.id);
                        this.notifyAvatarUpdate(io, publicProfile, room.id);
                        await roomDB.addMessage(this.connectSocketToRoom(socket, user.username, room.id));
                        room_joined = room;
                        status = true;
                        log_message = JoinRoomStatus.Join;
                        isPrivate = false;
                    } else {
                        log_message = JoinRoomStatus.AlreadyJoined;
                    }
                } else {
                    log_message = JoinRoomStatus.InvalidRoom;
                }
            }
        } else {
            log_message = JoinRoomStatus.InvalidUser;
        }
        const feedback: Feedback = {
            status: status,
            log_message: log_message
        };
        const joinRoomFeedback: JoinRoomFeedback = {
            feedback: feedback,
            room_joined: room_joined,
            isPrivate: isPrivate
        };
        
        return joinRoomFeedback;
    }

    public async leaveChatRoom(io: SocketIO.Server, socket: SocketIO.Socket, roomId: string, user: PrivateProfile | undefined): Promise<Feedback> {
        let privateRoom: Room | undefined = this.findPrivateRoom(roomId);
        let status: boolean = false;
        let log_message: LeaveRoomStatus;

        if (user) {
            if (privateRoom) {
                let socketIds: string[] = this.getSocketIds(io, roomId);
                if (socketIds.length == 1 && socketIds[0] == socket.id) {
                    this.deleteChatRoom(io, socket, roomId, user);
                    log_message = LeaveRoomStatus.LeaveAndDelete;
                } else {
                    privateRoom.messages.push(this.disconnectSocketFromRoom(socket, user.username, privateRoom.id));
                    log_message = LeaveRoomStatus.Leave;
                }
                status = true;
            } else {
                const room: Room | null = await roomDB.getRoom(roomId);
                if (room) {
                    if(user.rooms_joined.includes(room.id)) {
                        if(roomId == "General") {
                            log_message = LeaveRoomStatus.General;
                        } else {
                            await roomDB.addMessage(this.disconnectSocketFromRoom(socket, user.username, room.id));
                            user.rooms_joined.splice(user.rooms_joined.indexOf(room.id), 1);
                            await profileDB.leaveRoom(user.username, room.id);
                            status = true;
                            log_message = LeaveRoomStatus.Leave;
                        }
                    } else {
                        log_message = LeaveRoomStatus.NeverJoined;
                    }
                } else {
                    log_message = LeaveRoomStatus.InvalidRoom;
                }
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

    public async sendMessage(io: SocketIO.Server, clientMessage: ClientMessage, user: PrivateProfile): Promise<void> {
        let privateRoom: Room | undefined = this.findPrivateRoom(clientMessage.roomId);
        const message: Message = {
            username : user.username,
            content : ChatFilter.filter(clientMessage.content),
            date : Date.now(),
            roomId : clientMessage.roomId
        };
        
        if (privateRoom) {
            privateRoom.messages.push(message);
            io.in(message.roomId).emit("new_message", JSON.stringify(message));
        } else {
            const room: Room | null = await roomDB.getRoom(clientMessage.roomId);
            if (room) {
                // message.author.username == user.username
                if(user.rooms_joined.includes(clientMessage.roomId)) {
                    await roomDB.addMessage(message);
                    io.in(message.roomId).emit("new_message", JSON.stringify(message));
                } else {
                    console.log(user.username + " trying to send a message in " + clientMessage.roomId + " that he did not join");
                }
            } else {
                console.log(user?.username + " trying to send a message in " + clientMessage.roomId + " that does not exist");
            }
        }
    }

    public async invite(io: SocketIO.Server, socket: SocketIO.Socket, invitation: Invitation, users: Map<string, PrivateProfile>): Promise<Feedback> {
        const sender: PrivateProfile | undefined = users.get(socket.id);
        let receiver: PrivateProfile | undefined;
        let receiverSocketId: string | undefined;
        users.forEach((user: PrivateProfile, socketId: string) => {
            if (user.username == invitation.username) {
                receiver = user;
                receiverSocketId = socketId;
            }
        });

        let privateRoom: Room | undefined = this.findPrivateRoom(invitation.id);
        let status: boolean = false;
        let log_message: InviteStatus;

        if (sender) {
            if (receiver && receiverSocketId) {
                if(privateRoom || await roomDB.getRoom(invitation.id)) {
                    if (this.getSocketIds(io, invitation.id).includes(socket.id)) {
                        if (!this.getSocketIds(io, invitation.id).includes(receiverSocketId)) {
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

    private connectSocketToRoom(socket: SocketIO.Socket, username: string, roomId: string): Message {
        const message: Message = Admin.createAdminMessage(username + " joined the room.", roomId);
        socket.join(roomId, () => {
            socket.to(roomId).emit("new_message", JSON.stringify(message));
        });
        return message;
    }

    private disconnectSocketFromRoom(socket: SocketIO.Socket, username: string, roomId: string): Message {
        const message: Message = Admin.createAdminMessage( username + " left the room.", roomId);
        socket.leave(roomId, () => {
            socket.to(roomId).emit("new_message", JSON.stringify(message));
        });
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

    public findPrivateRoom(roomId: string): Room | undefined {
        return this.privateRooms.find(room => room.id == roomId);
    }

    public getUserPrivateRoomIds(username: string): string[] {
        let privateRoomIds: string[] = [];
        for (let room of this.privateRooms) {
            if (room.avatars.has(username)) {
                privateRoomIds.push(room.id);
            }
        }
        
        return privateRoomIds;
    }
}