import { MatchInstance, MatchMode } from "../models/matchMode";
import DrawPoint from "../models/drawPoint";
import { CreateMatch, MatchInfos } from "../models/match";
import { Feedback, CreateMatchFeedback } from "../models/feedback";
import Match from "./Match/matchAbstract";
import PrivateProfile from "../models/privateProfile";
import RandomMatchIdGenerator from "./IdGenerator/idGenerator";
import { ClientMessage } from "../models/message";
import ChatHandler from "./chatHandler";
import { CreateRoom } from "../models/room";

export default class MatchHandler {
    private currentMatches: Map<string, Match>;

    // Used for free draw testing.
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids

    public constructor() {
        this.currentMatches = new Map<string, Match>();
        this.observers = [];
    }
    
    public async createMatch( io: SocketIO.Server, socket: SocketIO.Socket, 
                    createMatch: CreateMatch, users: Map<string, PrivateProfile>, chatHandler: ChatHandler): Promise<CreateMatchFeedback> {
        const user: PrivateProfile | undefined = users.get(socket.id);
        let createMatchFeedback: CreateMatchFeedback = {
            feedback: { status: true, log_message: "Match created successfully." },
            matchId: ""
        }

        if (user) {
            const matchId: string = RandomMatchIdGenerator.generate();
            const matchRoom: CreateRoom = { id: matchId, isPrivate: true };
            const chatRoomFeedback: Feedback = await chatHandler.createChatRoom(io, socket, matchRoom, user);
            if (chatRoomFeedback.status) {
                this.currentMatches.set(matchId, MatchInstance.createMatch(socket.id, createMatch));
                socket.broadcast.emit("update_matches", this.getAvailableMatches(users));
            } else {
                createMatchFeedback.feedback = chatRoomFeedback;
            }
        } else {
            createMatchFeedback.feedback.status = false;
            createMatchFeedback.feedback.log_message = "You are not connected.";
        }

        return createMatchFeedback;
    }

    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, 
                    matchId: string, users: Map<string, PrivateProfile>, chatHandler: ChatHandler): Promise<Feedback> {
        const user: PrivateProfile | undefined = users.get(socket.id);
        const match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = {
            status: false,
            log_message: ""
        }

        if (user) {
            if (match) {
                feedback = (await chatHandler.joinChatRoom(io, socket, matchId, user)).feedback;
                if (feedback.status) {
                    feedback = match.joinMatch(socket.id);
                    socket.broadcast.emit("update_matches", this.getAvailableMatches(users));
                }
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not connected.";
        }   

        return feedback;
    }

    public async leaveMatch(io: SocketIO.Server, socket: SocketIO.Socket, 
                        matchId: string, users: Map<string, PrivateProfile>, chatHandler: ChatHandler): Promise<Feedback> {
        const user: PrivateProfile | undefined = users.get(socket.id);
        const match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = {
            status: false,
            log_message: ""
        }

        if (user) {
            if (match) {
                feedback = await chatHandler.leaveChatRoom(io, socket, matchId, user);
                if (feedback.status) {
                    feedback = match.leaveMatch(socket.id);
                    socket.broadcast.emit("update_matches", this.getAvailableMatches(users));
                }
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not connected.";
        }   

        return feedback;

    }

    public sendMessage(io: SocketIO.Server, socket: SocketIO.Socket, message: ClientMessage, user: PrivateProfile | undefined) {
        // TODO : check if it is a correct guess, or asking for a hint ("!hint"), and update the other players
    }

    public getAvailableMatches(users: Map<string, PrivateProfile>): MatchInfos[] {
        let availableMatches: MatchInfos[] = [];
        this.currentMatches.forEach((match: Match) => {
            if (!match.isStarted && match.mode !== MatchMode.sprintSolo) {
                availableMatches.push(match.getMatchInfos(users));
            }
        });
        return availableMatches;
    }

    /**
     * 
     * From here this code is used for free draw testing.
     *  
     */ 
    public enterFreeDrawTestRoom(socket: SocketIO.Socket): void {
        if (this.drawer) {
            this.observers.push(socket.id);
            socket.emit("observer");
        } else {
            this.drawer = socket.id;
            socket.emit("drawer");
        }
        socket.join("freeDrawRoomTest");
    }

    public leaveFreeDrawTestRoom(io: SocketIO.Server, socket: SocketIO.Socket): void {
        if (socket.id == this.drawer) {
            const newDrawer: string | undefined = this.observers.pop();
            if (newDrawer) {
                this.drawer = newDrawer;
                io.to(this.drawer).emit("drawer");
            }
        } else {
            this.observers.splice(this.observers.indexOf(socket.id), 1);
        }
        socket.leave("freeDrawRoomTest");
    }

    public drawTest(io: SocketIO.Server, socket: SocketIO.Socket, point: DrawPoint) {
        if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("drawPoint", point);
        }
    }
}