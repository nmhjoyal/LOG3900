import { MatchInstance } from "../models/matchMode";
import { CreateMatch, MatchInfos, TIME_LIMIT_MIN, TIME_LIMIT_MAX } from "../models/match";
import { Feedback, StartMatchFeedback, JoinRoomFeedback, CreateMatchFeedback } from "../models/feedback";
import Match from "./Match/matchAbstract";
import PrivateProfile from "../models/privateProfile";
import RandomMatchIdGenerator from "./IdGenerator/idGenerator";
import { ClientMessage } from "../models/message";
import { CreateRoom } from "../models/room";
import { GamePreview, Stroke, StylusPoint } from "../models/drawPoint";
import { VirtualDrawing } from "./Drawing/virtualDrawing";
import ChatHandler from "./chatHandler";
import Player from "../models/player";
import PublicProfile from "../models/publicProfile";

export default class MatchHandler {
    private currentMatches: Map<string, Match>;
    private chatHandler: ChatHandler;

    // Used for free draw testing.
    private drawer: string;         // Socket id
    private observers: string[];    // Socket ids
    private top: number;
    private previews: Map<string, VirtualDrawing>; // Key : socket.id or roomId, Value : virtual drawing

    public constructor() {
        this.currentMatches = new Map<string, Match>();
        this.observers = [];
        this.chatHandler = new ChatHandler();
        this.top = 0;
        this.previews = new Map<string, VirtualDrawing>();
    }
    
    public async createMatch(io: SocketIO.Server, socket: SocketIO.Socket, 
                    createMatch: CreateMatch, user: PrivateProfile | undefined): Promise<CreateMatchFeedback> {
        let createMatchFeedback: CreateMatchFeedback = { feedback: { status: false, log_message: "" }, matchId: ""};
        const NB_ROUNDS_MAX: number = MatchInstance.getMaxNbRounds(createMatch.matchMode);
        const NB_ROUNDS_MIN: number = MatchInstance.getMinNbRounds(createMatch.matchMode);

        if (user) {
            const matchId: string = RandomMatchIdGenerator.generate();
            const matchRoom: CreateRoom = { id: matchId, isPrivate: true };
            createMatchFeedback.feedback = await this.chatHandler.createChatRoom(io, socket, matchRoom, user);
            if (createMatchFeedback.feedback.status) {
                createMatchFeedback.feedback.status = false
                if (createMatch.timeLimit >= TIME_LIMIT_MIN && createMatch.timeLimit <= TIME_LIMIT_MAX) {
                    if (createMatch.nbRounds >= NB_ROUNDS_MIN && createMatch.nbRounds <= NB_ROUNDS_MAX) {
                        const match: Match = MatchInstance.createMatch(matchId, this.getPublicProfile(user), createMatch, this.chatHandler, io);
                        this.currentMatches.set(matchId, match);
                        io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
                        createMatchFeedback.feedback.status = true;
                        createMatchFeedback.feedback.log_message = "Match created successfully.";
                        createMatchFeedback.matchId = matchId;
                    } else {
                        createMatchFeedback.feedback.log_message = "The number of rounds has to be in between 1 and 10.";
                    }
                } else {
                    createMatchFeedback.feedback.log_message = "The time limit has to be in between 30 seconds and 2 minutes.";
                }
            }
        } else {
            createMatchFeedback.feedback.log_message = "You are not signed in.";
        }

        return createMatchFeedback;
    }

    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, matchId: string, user: PrivateProfile | undefined): Promise<JoinRoomFeedback> {
        const trimmedMatchId = matchId.replace(new RegExp('\"', 'g'), "");
        const match: Match | undefined = this.currentMatches.get(trimmedMatchId);
        let joinRoomFeedback: JoinRoomFeedback = { feedback: { status: false, log_message: "" }, room_joined: null, isPrivate: true };

        if (user) {
            if (match) {
                joinRoomFeedback = await match.joinMatch(io, socket, user);
                io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
            } else {
                joinRoomFeedback.feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            joinRoomFeedback.feedback.log_message = "You are not signed in.";
        }   

        return joinRoomFeedback;
    }

    public async leaveMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile | undefined): Promise<Feedback> {
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                const deleteMatch: boolean = await match.leaveMatch(io, socket, user);
                if (deleteMatch) this.currentMatches.delete(match.matchId);
                io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
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
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                feedback = match.addVirtualPlayer(user.username, io);
                io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not signed in.";
        }

        return feedback;
    }

    public removeVirtualPlayer(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile | undefined): Feedback {
        let feedback: Feedback = { status: false, log_message: "" };

        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                feedback = match.removeVirtualPlayer(user.username, io);
                io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
            } else {
                feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            feedback.log_message = "You are not signed in.";
        }

        return feedback;
    }

    public startMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile | undefined): StartMatchFeedback {
        let startMatchFeedback: StartMatchFeedback = { feedback: { status: false, log_message: "" } , nbRounds: 0 };

        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                startMatchFeedback = match.startMatch(user.username, io);
                if (startMatchFeedback.feedback.status) {
                    io.in(match.matchId).emit("match_started", JSON.stringify(startMatchFeedback));
                    io.emit("update_matches", JSON.stringify(this.getAvailableMatches()));
                    match.endTurn(io);
                } else {
                    socket.emit("match_started", JSON.stringify(startMatchFeedback));
                }
            } else {
                startMatchFeedback.feedback.log_message = "This match does not exist anymore.";
            }
        } else {
            startMatchFeedback.feedback.log_message = "You are not signed in.";
        }

        return startMatchFeedback;
    }

    public startTurn(io: SocketIO.Server, word: string, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if(match) {
                match.startTurn(io, word);
            } else {
                console.log("This match does not exist anymore");
            }
        } else {
            console.log("You are not signed in.");
        }
    }

    public sendMessage(io: SocketIO.Server, message: ClientMessage, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                this.chatHandler.sendMessage(io, message, user);
            } else {
                console.log("This match does not exist anymore.");
            }
        } else {
            console.log("You are not signed in.");
        }
    }

    public guess(io: SocketIO.Server, socket: SocketIO.Socket, guess: string, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                match.guess(io, socket, guess, user.username);
            } else {
                console.log("This match does not exist anymore.");
            }
        } else {
            console.log("You are not signed in.");
        }
    }

    public stroke(socket: SocketIO.Socket, stroke: Stroke, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                match.stroke(socket, stroke);
            } else {
                stroke.DrawingAttributes.Top = this.top++;
                socket.to("freeDrawRoomTest").emit("new_stroke", JSON.stringify(stroke));
            }
        }
    }
    
    public point(socket: SocketIO.Socket, point: StylusPoint, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                match.point(socket, point);
            } else {
                socket.to("freeDrawRoomTest").emit("new_point", JSON.stringify(point));
            }
        }
    }

    public eraseStroke(socket: SocketIO.Socket, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                match.eraseStroke(socket);
            } else {
                socket.to("freeDrawRoomTest").emit("new_erase_stroke");
            }
        }
    }

    public erasePoint(socket: SocketIO.Socket, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                match.erasePoint(socket);
            } else {
                socket.to("freeDrawRoomTest").emit("new_erase_point");
            }
        }
    }

    public clear(socket: SocketIO.Socket, user: PrivateProfile | undefined): void {
        if (user) {
            const match: Match | undefined = this.getMatchFromPlayer(user.username);
            if (match) {
                match.clear(socket);
            } else {
                let virtualDrawing: VirtualDrawing | undefined = this.previews.get(socket.id);
                if(virtualDrawing) {
                    virtualDrawing.clear(socket);
                };
            }
        }
    }

    
    private getPublicProfile(privateProfile: PrivateProfile): PublicProfile {
        return { username: privateProfile.username, avatar: privateProfile.avatar };
    }

    private getMatchFromPlayer(username: string): Match | undefined {
        let matchFound: Match | undefined;

        this.currentMatches.forEach((match: Match) => {
            if (match.getPlayer(username)) matchFound = match;
        });

        return matchFound;
    }

    public getPlayers(matchId: string): Player[] | undefined {
        return this.currentMatches.get(matchId)?.players;
    }

    public getAvailableMatches(): MatchInfos[] {
        let availableMatches: MatchInfos[] = [];
        this.currentMatches.forEach((match: Match, matchId: string) => {
            if (match.isEnded) {
                this.currentMatches.delete(matchId);
            } else {
                let matchInfos: MatchInfos | undefined = match.getMatchInfos();
                if (matchInfos) availableMatches.push(matchInfos);
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
        this.observers.push(socket.id);
        socket.emit("observer");
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
        let virtualDrawing: VirtualDrawing | undefined = this.previews.get(socket.id);
        if(!virtualDrawing) {
            virtualDrawing = new VirtualDrawing(null, 7.5);
            this.previews.set(socket.id, virtualDrawing);
        }
        virtualDrawing.preview(socket, gamePreview);
    }
}