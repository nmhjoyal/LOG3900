import { Feedback, StartMatchFeedback, JoinRoomFeedback } from "../../models/feedback";
import { MatchInfos, UpdateScore, CreateMatch } from "../../models/match";
import Player from "../../models/player";
import PublicProfile from "../../models/publicProfile";
import { MatchMode, MatchSettings } from "../../models/matchMode";
import ChatHandler from "../chatHandler";
import PrivateProfile from "../../models/privateProfile";
import { VirtualDrawing } from "../Drawing/virtualDrawing";
import { Drawing } from "../Drawing/drawing";
import { Stroke, StylusPoint } from "../../models/drawPoint";

export default abstract class Match {
    // Settings
    public matchId: string;
    protected mode: number;
    public players: Player[]; /* socketid, Player */
    protected nbRounds: number;
    protected timeLimit: number;
    protected currentWord: string;
    protected isStarted: boolean;
    protected chatHandler: ChatHandler;

    // Depends on the instance
    protected readonly ms: MatchSettings;

    // During the match
    protected timer: number;
    protected timeout: NodeJS.Timeout;          // setTimeout will be used for emitting end_turn and we will cancel it 
                                                // if there is an unexpected leave of a room or stoppage of a turn
    protected scores: Map<string, UpdateScore>  // Key: username, Value: score
    protected round: number;                    // In one round each player will draw one time
    protected currentPlayer: string ;           // Username 
    protected drawing: Drawing;
    protected virtualDrawing: VirtualDrawing;

    // Match methods
    public abstract startTurn(io: SocketIO.Server, chosenWord: string, isVirtual: boolean): void;
    protected abstract endTurn(io: SocketIO.Server): void;
    public abstract guess(io: SocketIO.Server, guess: string, username: string): Feedback;

    protected constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, matchSettings: MatchSettings) {
        this.players = [this.createPlayer(user, true, false)];
        this.isStarted = false;
        this.nbRounds = createMatch.nbRounds;
        this.matchId = matchId;
        this.mode = createMatch.matchMode;
        this.timeLimit = createMatch.timeLimit;
        this.chatHandler = chatHandler;
        this.ms = matchSettings;
    }

    /**
     * 
     * Functions used trough all match modes.
     * 
     */
    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile): Promise<JoinRoomFeedback> {
        let joinRoomFeedback: JoinRoomFeedback = { feedback: { status: false, log_message: "" }, room_joined: null, isPrivate: true };

        if (!this.isStarted) {
            if (this.players.length > this.ms.MAX_NB_PLAYERS) {
                if (this.getNbHumanPlayers() < this.ms.MAX_NB_HP) {
                    joinRoomFeedback = await this.chatHandler.joinChatRoom(io, socket, this.matchId, user);
                    this.players.push(this.createPlayer(user, false, false));
                    joinRoomFeedback.feedback.log_message = "You joined the match.";
                } else {
                    joinRoomFeedback.feedback.log_message = "You can not have more than" + this.ms.MAX_NB_HP + "human players in this mode.";
                }
            } else {
                joinRoomFeedback.feedback.log_message = "The maximum number of player is " + this.ms.MAX_NB_PLAYERS;
            }
        } else {
            joinRoomFeedback.feedback.log_message = "The match is already started.";
        }

        return joinRoomFeedback;
    }

    public async leaveMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile): Promise<boolean> {
        const player: Player | undefined = this.getPlayer(user.username);
        let deleteMatch: boolean = false;

        if (player) {
            await this.chatHandler.leaveChatRoom(io, socket, this.matchId, user);
            if (this.getNbHumanPlayers() > 1) {
                this.players.splice(this.players.indexOf(player), 1);
                if (this.isStarted) {
                    if (player.user.username == this.currentPlayer) {
                        this.endTurn(io);
                    }
                } else {
                    if (player.isHost) {
                        for(let player of this.players) {
                            if (!player.isHost && !player.isVirtual) {
                                player.isHost = true;
                                io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
                                break;
                            }
                        }
                    } 
                }
            } else {
                deleteMatch = true;
            }
        }

        return deleteMatch;
    }

    public addVirtualPlayer(username: string, io: SocketIO.Server): Feedback {
        const player: Player | undefined = this.getPlayer(username);
        let feedback: Feedback = { status: false, log_message: "" };

        if (player) {
            if (player.isHost) {
                if (this.players.length > this.ms.MAX_NB_PLAYERS) {
                    if (this.getNbVirtualPlayers() < this.ms.MAX_NB_VP) {
                        /* EVENTUALLY, GENERATE RANDOM VP, also need to check if it is already in the players array (so we dont have two identical VP)*/ 
                        const randomVP: PublicProfile = { username: "Mr Avocado", avatar: "AVOCADO" };
                        this.players.push(this.createPlayer(randomVP, false, true));
                        this.chatHandler.findPrivateRoom(this.matchId)?.avatars.set(randomVP.username, randomVP.avatar);
                        this.chatHandler.notifyAvatarUpdate(io, randomVP, this.matchId);
                        io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
                        feedback.status = true;
                        feedback.log_message = "A virtual player was added.";
                    } else {
                        feedback.log_message = "You can not have more than" + this.ms.MAX_NB_VP + "virtual players in this mode.";
                    }
                } else {
                    feedback.log_message = "The maximum number of player is " + this.ms.MAX_NB_PLAYERS;
                }
            } else {
                feedback.log_message = "You are not the host. Only the host can add a virtual player.";
            }
        } else {
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback;
    }

    public removeVirtualPlayer(username: string, io: SocketIO.Server): Feedback {
        const player: Player | undefined = this.getPlayer(username);
        let feedback: Feedback = { status: false, log_message: "" };

        if (player) {
            if (player.isHost) {
                if (this.getNbVirtualPlayers() > this.ms.MIN_NB_VP) {
                    for(let i: number = this.players.length - 1; i > -1; i--) {
                        if(this.players[i].isVirtual) {
                            this.players.splice(i, 1);
                            break;
                        }
                    }
                    io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
                    feedback.status = true;
                    feedback.log_message = "A virtual player was removed."
                } else {
                    feedback.log_message = "You can not have less than" + this.ms.MIN_NB_VP + "virtual players in this mode."
                }
            } else {
                feedback.log_message = "You are not the host. Only the host can remove a virtual player.";
            }
        } else {
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback;
    }

    public startMatch(username: string, io: SocketIO.Server): StartMatchFeedback {
        const player: Player | undefined = this.getPlayer(username);
        let startMatchFeedback: StartMatchFeedback = { feedback : { status: false, log_message: "" }, nbRounds : 0 };

        if (player) {
            if (player.isHost) {
                const nbHumanPlayers: number = this.getNbHumanPlayers();
                if (nbHumanPlayers > this.ms.MIN_NB_HP || nbHumanPlayers < this.ms.MAX_NB_HP) {
                    const nbVirtualPlayers: number = this.getNbVirtualPlayers();
                    if (nbVirtualPlayers > this.ms.MIN_NB_VP || nbVirtualPlayers < this.ms.MAX_NB_VP) {
                        this.initMatch();
                        this.endTurn(io);
                        startMatchFeedback.nbRounds = this.nbRounds;
                        startMatchFeedback.feedback.status = true;
                        startMatchFeedback.feedback.log_message = "Match is starting...";
                    } else {
                        (this.ms.MIN_NB_VP === this.ms.MAX_NB_VP) ?
                        startMatchFeedback.feedback.log_message = "The number of virtual players needs to be exactly " + this.ms.MIN_NB_VP:
                        startMatchFeedback.feedback.log_message = "The number of virtual players needs to be in between " + this.ms.MIN_NB_VP + " and " + this.ms.MAX_NB_VP;
                    }
                } else { 
                    (this.ms.MIN_NB_HP === this.ms.MAX_NB_HP) ?
                    startMatchFeedback.feedback.log_message = "The number of human players needs to be exactly " + this.ms.MIN_NB_HP:
                    startMatchFeedback.feedback.log_message = "The number of human players needs to be in between " + this.ms.MIN_NB_HP + " and " + this.ms.MAX_NB_HP;
                }
            } else {
                startMatchFeedback.feedback.log_message = "You are not the host. Only the host can start the match.";
            }
        } else {
            startMatchFeedback.feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return startMatchFeedback;
    }

    protected stroke(socket: SocketIO.Socket, stroke: Stroke): void {
        this.drawing.stroke(socket, stroke);
    }
    
    protected point(socket: SocketIO.Socket, point: StylusPoint): void {
        this.drawing.point(socket, point);
    }

    protected eraseStroke(socket: SocketIO.Socket): void {
        this.drawing.eraseStroke(socket);
    }

    protected erasePoint(socket: SocketIO.Socket): void {
        this.drawing.erasePoint(socket);
    }

    protected clear(socket: SocketIO.Socket): void {
        this.drawing.clear(socket);
    }
    
    protected endMatch(): void {

    }

    protected initScores(): void {
        this.scores = new Map<string, UpdateScore>();
        for (let player of this.players) {
            if(!player.isVirtual) {
                this.scores.set(player.user.username, { scoreTotal: 0, scoreTurn: 0 });
            }
        }
    }

    protected initMatch(): void {
        this.isStarted = true;
        this.drawing = new Drawing(this.matchId);
        this.virtualDrawing = new VirtualDrawing(this.matchId, this.timeLimit);
        this.currentPlayer = this.players[0].user.username;
        this.round = 1;
        this.initScores();
    }

    protected resetScoresTurn(): void {
        this.scores.forEach((score: UpdateScore) => {
            score.scoreTurn = 0;
        });
    }

    protected everyoneHasGuessed(): boolean {
        let everyoneHasGuessed: boolean = true;

        this.scores.forEach((score: UpdateScore, username: string) => {
            if (score.scoreTurn == 0 && username != this.currentPlayer) everyoneHasGuessed = false;
        });

        return everyoneHasGuessed;
    }

    protected updateScore(username: string, score: number): void {
        let playerScore: UpdateScore | undefined = this.scores.get(username);

        if (playerScore) {
            const oldScore: number = playerScore.scoreTotal;
            const updatedScore: UpdateScore = {
                scoreTotal: oldScore + score,
                scoreTurn: score
            };
            this.scores.set(username, updatedScore);
        } else {
            console.log("error while updating score of : " + username);       
        }
    }

    protected getNbVirtualPlayers(): number {
        let count: number = 0;

        for (let player of this.players) {
            if (player.isVirtual) count++;
        }
        
        return count;
    }

    protected getNbHumanPlayers(): number {
        let count: number = 0;

        for (let player of this.players) {
            if (!player.isVirtual) count++;
        }

        return count;
    }

    public getMatchInfos(): MatchInfos | undefined {
        let matchInfos: MatchInfos | undefined;
        
        if (!this.isStarted && this.mode !== MatchMode.sprintSolo) { // maybe this.players.length < this.maxPlayers?    
            let userInfos: PublicProfile[] = [];
            let host: string = "";

            for (let player of this.players) {
                if (player.isHost) host = player.user.username; 
                userInfos.push(player.user);
            }

            matchInfos = {
                matchId: this.matchId,
                host: host,
                matchMode: this.mode,
                nbRounds: this.nbRounds,
                timeLimit: this.timeLimit,
                players: userInfos
            };
        }

        return matchInfos;
    }

    public getPlayer(username: string): Player | undefined {
        return this.players.find(player => username == player.user.username)
    }

    protected createPlayer(user: PublicProfile, isHost: boolean, isVirtual: boolean): Player {
        return {
            user: user,
            isHost: isHost,
            isVirtual: isVirtual
        };
    }
}