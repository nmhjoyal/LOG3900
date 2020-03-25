import { MatchInstance } from "../models/matchMode";
import { CreateMatch, MatchInfos, StartMatch } from "../models/match";
import { Feedback, StartMatchFeedback, JoinRoomFeedback } from "../models/feedback";
import Match from "./Match/matchAbstract";
import PrivateProfile from "../models/privateProfile";
import RandomMatchIdGenerator from "./IdGenerator/idGenerator";
import { ClientMessage } from "../models/message";
import { CreateRoom } from "../models/room";
import PublicProfile from "../models/publicProfile";
import { GamePreview, Stroke, StylusPoint } from "../models/drawPoint";
import { VirtualDrawing } from "./Drawing/virtualDrawing";
import ChatHandler from "./chatHandler";

export default class MatchHandler {
    private currentMatches: Map<string, Match>;
    private chatHandler: ChatHandler;

    // Used for free draw testing.
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids
    private top: number;
    private previews: VirtualDrawing[];

    public constructor() {
        this.currentMatches = new Map<string, Match>();
        this.observers = [];
        this.chatHandler = new ChatHandler();
        this.top = 0;
        this.previews = [];
    }
    
    public async createMatch(io: SocketIO.Server, socket: SocketIO.Socket, 
                    createMatch: CreateMatch, user: PrivateProfile | undefined): Promise<Feedback> {
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            const matchId: string = RandomMatchIdGenerator.generate();
            const matchRoom: CreateRoom = { id: matchId, isPrivate: true };
            const chatRoomFeedback: Feedback = await this.chatHandler.createChatRoom(io, socket, matchRoom, user);
            if (chatRoomFeedback.status) {
                this.currentMatches.set(matchId, MatchInstance.createMatch(matchId, socket.id, {username: user.username, avatar: user.avatar}, createMatch, this.chatHandler));
                io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
                feedback.log_message = "Match created successfully.";
                feedback.status = true;
            } else {
                feedback = chatRoomFeedback;
            }
        } else {
            feedback.log_message = "You are not signed in.";
        }

        return feedback;
    }

    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, 
                    matchId: string, user: PrivateProfile | undefined): Promise<JoinRoomFeedback> {
        const match: Match | undefined = this.currentMatches.get(matchId);
        let joinRoomFeedback: JoinRoomFeedback = { feedback: { status: false, log_message: "" }, room_joined: null, isPrivate: true };

        if (user) {
            if (match) {
                joinRoomFeedback = await match.joinMatch(io, socket, user);
                socket.broadcast.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
            } else {
                joinRoomFeedback.feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            joinRoomFeedback.feedback.log_message = "You are not signed in.";
        }   

        return joinRoomFeedback;
    }

    public async leaveMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile | undefined): Promise<Feedback> {
        const match: Match | undefined = this.getMatchFromPlayer(socket.id);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                const deleteMatch: boolean = await match.leaveMatch(io, socket, user);
                if (deleteMatch) this.currentMatches.delete(match.matchId);
                socket.broadcast.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
                feedback.status = true;
                feedback.log_message = "You left the match.";
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not signed in.";
        }   

        return feedback;

    }

    public addVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile | undefined): Feedback {
        const match: Match | undefined = this.getMatchFromPlayer(socket.id);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = match.addVirtualPlayer(socket.id, io);
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not signed in.";
        }

        return feedback;
    }

    public removeVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile | undefined): Feedback {
        const match: Match | undefined = this.getMatchFromPlayer(socket.id);
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            if (match) {
                feedback = match.removeVirtualPlayer(socket.id, io);
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not signed in.";
        }

        return feedback;
    }

    public startMatch(io: SocketIO.Server, socket: SocketIO.Socket, startMatch: StartMatch, user: PrivateProfile | undefined): StartMatchFeedback {
        const match: Match | undefined = this.currentMatches.get(startMatch.matchId);
        let startMatchFeedback: StartMatchFeedback = { feedback: { status: false, log_message: "" } , nbRounds: 0 };

        if (user) {
            if (match) {
                startMatchFeedback = match.startMatch(socket.id, io, startMatch);
            } else {
                startMatchFeedback.feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            startMatchFeedback.feedback.log_message = "You are not signed in.";
        }

        return startMatchFeedback;
    }

    public sendMessage(io: SocketIO.Server, socket: SocketIO.Socket, message: ClientMessage, user: PrivateProfile | undefined): void {
        // TODO : check if it is a correct guess, or asking for a hint ("!hint"), and update the other players
    }

    private getMatchFromPlayer(socketId: string): Match | undefined {
        let matchFound: Match | undefined;

        this.currentMatches.forEach((match: Match) => {
            if (match.players.has(socketId)) matchFound = match;
        });

        return matchFound;
    }

    public getPlayers(matchId: string): PublicProfile[] | undefined {
        return this.currentMatches.get(matchId)?.getPlayersPublicProfile();
    }

    public getAvailableMatches(): MatchInfos[] {
        let availableMatches: MatchInfos[] = [];
        this.currentMatches.forEach((match: Match) => {
            let matchInfos: MatchInfos | undefined = match.getMatchInfos();
            if (matchInfos) availableMatches.push(matchInfos);
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

    public stroke(io: SocketIO.Server, socket: SocketIO.Socket, stroke: Stroke): void {
        // if (socket.id == this.drawer) {
            stroke.DrawingAttributes.Top = this.top++;
            socket.to("freeDrawRoomTest").emit("new_stroke", JSON.stringify(stroke));
        // }
    }

    public eraseStroke(io: SocketIO.Server, socket: SocketIO.Socket): void {
        socket.to("freeDrawRoomTest").emit("new_erase_stroke");
    }

    public erasePoint(io: SocketIO.Server, socket: SocketIO.Socket): void {
        socket.to("freeDrawRoomTest").emit("new_erase_point");
    }

    public point(io: SocketIO.Server, socket: SocketIO.Socket, point: StylusPoint): void {
        // if (socket.id == this.drawer) {
        socket.to("freeDrawRoomTest").emit("new_point", JSON.stringify(point));
        // }
    }

    public clear(io: SocketIO.Server, socket: SocketIO.Socket): void {
        // Pour preview seulement
        let virtualDrawing: VirtualDrawing | undefined = this.previews.find(drawing => socket.id == drawing.getId());
        if(virtualDrawing) {
            console.log("clear");
            virtualDrawing.clear();
        };
    }

    public async getDrawing(io: SocketIO.Server): Promise<void> {
        /*
        const game: Game = await gameDB.getRandomGame();
        let virtualDrawing: VirtualDrawing | undefined = this.previews.find(drawing => "freeDrawRoomTest" == drawing.getId());
        if(!virtualDrawing) {
            virtualDrawing = new VirtualDrawing("froomDrawRoomTest", io, 20);
            this.previews.push(virtualDrawing);
        }
        virtualDrawing.draw(game.drawing, game.level);
        */
    }

    // previewHandler.ts
    public async preview(socket: SocketIO.Socket, gamePreview: GamePreview): Promise<void> {
        let virtualDrawing: VirtualDrawing | undefined = this.previews.find(drawing => socket.id == drawing.getId());
        if(!virtualDrawing) {
            virtualDrawing = new VirtualDrawing(null, socket, 7.5);
            this.previews.push(virtualDrawing);
        }
        virtualDrawing.preview(gamePreview);
    }
}