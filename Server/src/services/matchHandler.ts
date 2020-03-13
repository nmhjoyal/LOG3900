import { MatchInstance } from "../models/matchMode";
import DrawPoint from "../models/drawPoint";
import { CreateMatch } from "../models/match";
import { Feedback, MatchCreationFeedBack } from "../models/feedback";
import Match from "./Match/match_General";
import { serverHandler } from "./serverHandler";
import PrivateProfile from "../models/privateProfile";
import RandomIdGenerator from "./IdGenerator/idGen";

export default class MatchHandler {
    private currentMatches: Map<string, Match>;

    // Used for free draw testing.
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids

    public constructor() {
        this.currentMatches = new Map<string, Match>();
        this.observers = [];
    }
    
    public createMatch(socketId: string, createMatch: CreateMatch): MatchCreationFeedBack {
        const user: PrivateProfile | undefined = serverHandler.users.get(socketId);
        let feedback: Feedback = {
            status: true,
            log_message: "Match created successfully."
        };
        let creationfeedback: MatchCreationFeedBack = {
            feedback: feedback,
            host: null,
            matchMode: null,
            nbRounds: null
        };

        if (user) {
            let newMatch: Match = MatchInstance.createMatch(createMatch.matchMode, socketId, createMatch.nbRounds);
            this.currentMatches.set(RandomIdGenerator.generate(), newMatch);
            creationfeedback.host = user.username;
            creationfeedback.matchMode = createMatch.matchMode;
            creationfeedback.nbRounds = createMatch.nbRounds;
        } else {
            creationfeedback.feedback.status = false;
            creationfeedback.feedback.log_message = "You are not connected.";
        }

        return creationfeedback;
    }

    public joinMatch(socketId: string, matchId: string): Feedback {
        let match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = {
            status: true,
            log_message: "You joined the match."
        }

        if (match) {
            feedback = match.joinMatch(socketId);
        } else {
            feedback.status = false;
            feedback.log_message = "Could not join the match."
        }

        return feedback;
    }

    /**
     * 
     * From here this code is used for free draw testing.
     *  
     * */ 
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