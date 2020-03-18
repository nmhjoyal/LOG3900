import { MatchInstance } from "../models/matchMode";
import DrawPoint from "../models/drawPoint";
import { CreateMatch, MatchInfos } from "../models/match";
import { Feedback} from "../models/feedback";
import Match from "./Match/match_General";
import PrivateProfile from "../models/privateProfile";
import PublicProfile from "../models/publicProfile";
import RandomMatchIdGenerator from "./IdGenerator/idGenerator";
import { ClientMessage } from "../models/message";

export default class MatchHandler {
    private currentMatches: Map<string, Match>;

    // Used for free draw testing.
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids

    public constructor() {
        this.currentMatches = new Map<string, Match>();
        this.observers = [];
    }
    
    public createMatch(socket: SocketIO.Socket, createMatch: CreateMatch, user: PrivateProfile | undefined): Feedback {
        let feedback: Feedback = {
            status: true,
            log_message: "Match created successfully."
        };

        if (user) {
            let randomId: string = RandomMatchIdGenerator.generate();
            let newMatch: Match = MatchInstance.createMatch(socket, randomId, createMatch);
            this.currentMatches.set(randomId, newMatch);
            socket.emit("update_matches", this.getAllAvailbaleMatches());
        } else {
            feedback.status = false;
            feedback.log_message = "You are not connected.";
        }

        return feedback;
    }

    public joinMatch(socket: SocketIO.Socket, matchId: string, user: PrivateProfile | undefined): Feedback {
        let match: Match | undefined = this.currentMatches.get(matchId);
        let feedback: Feedback = {
            status: false,
            log_message: "You joined the match."
        }

        if (user) {
            if (match) {
                if (!match.isStarted) {
                    let publicProfile: PublicProfile = {
                        username: user.username,
                        avatar: user.avatar
                    };
                    feedback = match.joinMatch(socket, publicProfile);
                } else {
                    feedback.log_message = "This match is already started.";
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

    private getAllAvailbaleMatches(): MatchInfos[] {
        let availableMatches: MatchInfos[] = [];
        this.currentMatches.forEach((match: Match) => {
            if (!match.isStarted) {
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

    public drawTest(io: SocketIO.Server, socket: SocketIO.Socket, point: DrawPoint) {
        if (socket.id == this.drawer) {
            socket.to("freeDrawRoomTest").emit("drawPoint", point);
        }
    }
}