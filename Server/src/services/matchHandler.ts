import { MatchInstance, MatchMode } from "../models/matchMode";
import { CreateMatch, MatchInfos, StartMatch } from "../models/match";
import { Feedback, CreateMatchFeedback } from "../models/feedback";
import Match from "./Match/matchAbstract";
import PrivateProfile from "../models/privateProfile";
import RandomMatchIdGenerator from "./IdGenerator/idGenerator";
import { ClientMessage } from "../models/message";
import ChatHandler from "./chatHandler";
import { CreateRoom } from "../models/room";
import PublicProfile from "../models/publicProfile";
import { Trace, Point, Game, GamePreview } from "../models/drawPoint";
import { VirtualPlayer } from "./Drawing/virtualPlayer";
import { gameDB } from "./Database/gameDB";

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
                    createMatch: CreateMatch, user: PrivateProfile | undefined, chatHandler: ChatHandler): Promise<CreateMatchFeedback> {
        let createMatchFeedback: CreateMatchFeedback = {
            feedback: { status: false, log_message: "Match created successfully." },
            matchId: ""
        }

        if (user) {
            const matchId: string = RandomMatchIdGenerator.generate();
            const matchRoom: CreateRoom = { id: matchId, isPrivate: true };
            const chatRoomFeedback: Feedback = await chatHandler.createChatRoom(io, socket, matchRoom, user);
            if (chatRoomFeedback.status) {
                this.currentMatches.set(matchId, MatchInstance.createMatch(matchId, socket.id, {username: user.username, avatar: user.avatar}, createMatch));
                socket.broadcast.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
                createMatchFeedback.feedback.status = true
                createMatchFeedback.matchId = matchId
            } else {
                createMatchFeedback.feedback = chatRoomFeedback;
            }
        } else {
            createMatchFeedback.feedback.log_message = "You are not connected.";
        }

        return createMatchFeedback;
    }

    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, 
                    matchId: string, user: PrivateProfile | undefined, chatHandler: ChatHandler): Promise<Feedback> {
        const match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = (await chatHandler.joinChatRoom(io, socket, matchId, user)).feedback;
                if (feedback.status) {
                    feedback = match.joinMatch(socket.id, {username: user.username, avatar: user.avatar});
                    socket.broadcast.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
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
                        matchId: string, user: PrivateProfile | undefined, chatHandler: ChatHandler): Promise<Feedback> {
        const match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = await chatHandler.leaveChatRoom(io, socket, matchId, user);
                if (feedback.status) {
                    feedback = match.leaveMatch(socket.id);
                    socket.broadcast.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
                }
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not connected.";
        }   

        return feedback;

    }

    public addVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket, 
                        matchId: string, user: PrivateProfile | undefined, chatHandler: ChatHandler): Feedback {
        const match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = match.addVirtualPlayer(socket.id, io);
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not connected.";
        }

        return feedback;
    }

    public removeVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket, 
                        matchId: string, user: PrivateProfile | undefined, chatHandler: ChatHandler): Feedback {
        const match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = match.removeVirtualPlayer(socket.id, io);
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not connected.";
        }

        return feedback;
    }

    public startMatch(io: SocketIO.Server, socket: SocketIO.Socket, startMatch: StartMatch, user: PrivateProfile | undefined): Feedback {
        const match: Match | undefined = this.currentMatches.get(startMatch.matchId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = match.startMatch(socket.id, io, startMatch);
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not connected.";
        }

        return feedback;
    }

    public sendMessage(io: SocketIO.Server, socket: SocketIO.Socket, message: ClientMessage, user: PrivateProfile | undefined): void {
        // TODO : check if it is a correct guess, or asking for a hint ("!hint"), and update the other players
    }

    public getPlayers(matchId: string): PublicProfile[] | undefined {
        return this.currentMatches.get(matchId)?.getPlayersPublicProfile();
    }

    public getAvailableMatches(): MatchInfos[] {
        let availableMatches: MatchInfos[] = [];
        this.currentMatches.forEach((match: Match) => {
            if (!match.isStarted && match.mode !== MatchMode.sprintSolo) {
                availableMatches.push(match.getMatchInfos());
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

    public startTrace(io: SocketIO.Server, socket: SocketIO.Socket, trace: Trace): void {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("new_trace", JSON.stringify(trace));
        // }
    }

    public drawTest(io: SocketIO.Server, socket: SocketIO.Socket, point: Point): void {
        // if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("new_point", JSON.stringify(point));
        // }
    }

    public async getDrawing(io: SocketIO.Server): Promise<void> {
        const game: Game = await gameDB.getRandomGame();
        console.log(JSON.stringify(game));
        const virtualPlayer: VirtualPlayer = new VirtualPlayer("bot", "freeDrawRoomTest", io);
        virtualPlayer.setTimePerRound(10);
        virtualPlayer.draw(game);
    }

    // previewHandler.ts
    public async preview(socket: SocketIO.Socket, gamePreview: GamePreview): Promise<void> {
        const virtualPlayer: VirtualPlayer = new VirtualPlayer("bot", null, socket);
        virtualPlayer.setTimePerRound(5);
        virtualPlayer.preview(gamePreview);
    }
}