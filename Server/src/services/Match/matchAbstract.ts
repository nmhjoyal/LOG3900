import { Feedback, StartMatchFeedback, JoinRoomFeedback } from "../../models/feedback";
import { MatchInfos, UpdateScore, CreateMatch, EndTurn, StartTurn, Score } from "../../models/match";
import Player from "../../models/player";
import PublicProfile from "../../models/publicProfile";
import { MatchMode, MatchSettings } from "../../models/matchMode";
import ChatHandler from "../chatHandler";
import PrivateProfile from "../../models/privateProfile";
import { VirtualDrawing } from "../Drawing/virtualDrawing";
import { Drawing } from "../Drawing/drawing";
import { Stroke, StylusPoint } from "../../models/drawPoint";
import RandomWordGenerator from "../WordGenerator/wordGenerator";
import Admin from "../../models/admin";
import { Message } from "../../models/message";
import { gameDB } from "../Database/gameDB";
import VirtualPlayer from "../VirtualPlayer/virtualPlayer";

export default abstract class Match {
    // Settings
    public matchId: string;
    public isEnded: boolean;
    public players: Player[]; /* socketid, Player */
    protected mode: number;
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
    protected drawer: string;                   // Username 
    protected drawing: Drawing;
    protected virtualDrawing: VirtualDrawing;
    protected virtualPlayer: VirtualPlayer;

    // Match methods
    public abstract startTurn(io: SocketIO.Server, socket: SocketIO.Socket | null, chosenWord: string, isVirtual: boolean): void;
    protected abstract endTurn(io: SocketIO.Server, drawerLeft: boolean): void;
    public abstract guess(io: SocketIO.Server, guess: string, username: string): Feedback;

    protected constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, matchSettings: MatchSettings) {
        this.players = [this.createPlayer(user)];
        this.isStarted = false;
        this.isEnded = false;
        this.nbRounds = createMatch.nbRounds;
        this.matchId = matchId;
        this.mode = createMatch.matchMode;
        this.timeLimit = createMatch.timeLimit;
        this.chatHandler = chatHandler;
        this.ms = matchSettings;
        this.virtualPlayer = new VirtualPlayer();
    }

    /**
     * 
     * Functions used trough all match modes.
     * 
     */
    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile): Promise<JoinRoomFeedback> {
        let joinRoomFeedback: JoinRoomFeedback = { feedback: { status: false, log_message: "" }, room_joined: null, isPrivate: true };

        if (!this.isStarted) {
            if (this.players.length < this.ms.MAX_NB_PLAYERS) {
                if (this.getNbHumanPlayers() < this.ms.MAX_NB_HP) {
                    joinRoomFeedback = await this.chatHandler.joinChatRoom(io, socket, this.matchId, user);
                    this.players.push(this.createPlayer(user));
                    io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
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
                if (this.isStarted) {
                    if (this.getNbHumanPlayers() - 1 > this.ms.MIN_NB_HP) {
                        // after the match is started the host is not important.
                        if (player.user.username == this.drawer) {
                            let oldDrawer: Player | undefined = this.getPlayer(this.drawer);
                            if (oldDrawer) {
                                this.assignDrawer(oldDrawer);
                                this.endTurn(io, true);
                            }
                        }
                    } else {
                        this.endMatch(io);
                        deleteMatch = true;
                    }
                } else {
                    if (this.getNbHumanPlayers() > 1) { // otherwise assignHost wouldnt work.
                        if (this.isHost(player)) {
                            this.assignHost();
                        }
                    } else {
                        deleteMatch = true;
                    }
                }
                this.players.splice(this.players.indexOf(player), 1);
                io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
        }

        return deleteMatch;
    }

    public addVirtualPlayer(username: string, io: SocketIO.Server): Feedback {
        const player: Player | undefined = this.getPlayer(username);
        let feedback: Feedback = { status: false, log_message: "" };
        if (player) {
            if (this.isHost(player)) {
                if (this.players.length < this.ms.MAX_NB_PLAYERS) {
                    if (this.getNbVirtualPlayers() < this.ms.MAX_NB_VP) {
                        this.addVP(io);
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
            if (this.isHost(player)) {
                if (this.getNbVirtualPlayers() > this.ms.MIN_NB_VP) {
                    this.removeVP();
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
            if (this.isHost(player)) {
                const nbHumanPlayers: number = this.getNbHumanPlayers();
                if (nbHumanPlayers > this.ms.MIN_NB_HP || nbHumanPlayers < this.ms.MAX_NB_HP) {
                    const nbVirtualPlayers: number = this.getNbVirtualPlayers();
                    if (nbVirtualPlayers > this.ms.MIN_NB_VP || nbVirtualPlayers < this.ms.MAX_NB_VP) {
                        this.initMatch();
                        this.endTurn(io, false);
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

    protected async endTurnGeneral(io: SocketIO.Server): Promise<void> {
        const endTurn: EndTurn = this.createEndTurn();

        if (this.currentWord != "") {
            const message: Message = Admin.createAdminMessage("The word was " + this.currentWord, this.matchId);
            io.in(this.matchId).emit("new_message", JSON.stringify(message));
        }

        io.in(this.matchId).emit("turn_ended", JSON.stringify(endTurn));
        
        this.resetScoresTurn();
        this.currentWord = "";
        
        if (this.getPlayer(this.drawer)?.isVirtual) {
            let word: string;
            setTimeout(() => {
                this.startTurn(io, null, word, true);
            }, 5000);
            word = await gameDB.getRandomWord();
        }
    }

    protected endMatch(io: SocketIO.Server): void {
        // compile game stats for the players and the standings.
        // notify everyone that the game is ended.
         
        this.isEnded = true;
    }

    /**
     * 
     * 
     * Drawing in real time.
     * 
     * 
     */
    public stroke(socket: SocketIO.Socket, stroke: Stroke): void {
        this.drawing.stroke(socket, stroke);
    }
    
    public point(socket: SocketIO.Socket, point: StylusPoint): void {
        this.drawing.point(socket, point);
    }

    public eraseStroke(socket: SocketIO.Socket): void {
        this.drawing.eraseStroke(socket);
    }

    public erasePoint(socket: SocketIO.Socket): void {
        this.drawing.erasePoint(socket);
    }

    public clear(socket: SocketIO.Socket): void {
        this.drawing.clear(socket);
    }

    protected isHost(player: Player): boolean {
        return this.players.indexOf(player) == 0;
    }

    protected initScores(): void {
        this.scores = new Map<string, UpdateScore>();
        for (let player of this.players) {
            if(!player.isVirtual) {
                const updateScore: UpdateScore = {
                    scoreTotal: 0,
                    scoreTurn: 0
                }
                this.scores.set(player.user.username, updateScore);
            }
        }
    }

    protected initMatch(): void {
        this.isStarted = true;
        this.drawing = new Drawing(this.matchId);
        this.virtualDrawing = new VirtualDrawing(this.matchId, this.timeLimit);

        // Init to the last player on round 0 so it resets in endTurn for round 1 with first player.
        this.drawer = this.players[this.players.length - 1].user.username;
        this.round = 0;
        
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
            if (score.scoreTurn == 0 && username != this.drawer) everyoneHasGuessed = false;
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

    protected assignDrawer(oldDrawer: Player) {
        const currentIndex: number = this.players.indexOf(oldDrawer);
        if (currentIndex == this.players.length - 1) {
            this.drawer = this.players[0].user.username;
            this.round++;
        } else {
            this.drawer = this.players[currentIndex + 1].user.username;
        }
    }

    protected assignHost(): void {
        // Place the new host at the beginning of the array.
        this.players.splice(0, 0, this.players.splice(this.findNewHostIndex(), 1)[0])
    }

    protected findNewHostIndex(): number {
        return this.players.findIndex(player => !player.isVirtual);
    }

    protected addVP(io: SocketIO.Server): void {
        const randomVP: Player = this.virtualPlayer.create();
        this.players.push(randomVP);
        this.chatHandler.findPrivateRoom(this.matchId)?.avatars.set(randomVP.user.username, randomVP.user.avatar);
        this.chatHandler.notifyAvatarUpdate(io, randomVP.user, this.matchId);
    }

    protected removeVP(): void {
        for(let i: number = this.players.length - 1; i >= 0; i--) {
            let player: Player = this.players[i];
            if(player.isVirtual) {
                this.virtualPlayer.newAvailableVP(player.user.username);
                this.players.splice(i, 1);
                break;
            }
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

            for (let player of this.players) {
                userInfos.push(player.user);
            }

            matchInfos = this.createMatchInfos(this.players[0].user.username, userInfos);
        }

        return matchInfos;
    }

    public getPlayer(username: string): Player | undefined {
        return this.players.find(player => username == player.user.username);
    }

    protected createMatchInfos(host: string, userInfos: PublicProfile[]): MatchInfos {
        return {
            matchId: this.matchId,
            host: host,
            matchMode: this.mode,
            nbRounds: this.nbRounds,
            timeLimit: this.timeLimit,
            players: userInfos
        };
    }

    protected createPlayer(user: PublicProfile): Player {
        return {
            user: user,
            isVirtual: false
        };
    }

    protected createStartTurn(word: string, isDrawer: boolean): StartTurn {
        return { 
            timeLimit: this.timeLimit,
            word: isDrawer? word: word.replace(/[a-z]/gi, '_ ')
        };
    }

    protected getScores(): Score[] {
        let scores: Score[] = [];
        this.scores.forEach((updateScore: UpdateScore, username: string) => {
            scores.push({
                username: username,
                updateScore: updateScore
            })
        });
        return scores;
    }

    protected createEndTurn(): EndTurn {
        return {
            currentRound: this.round,
            choices: RandomWordGenerator.generateChoices(),
            drawer: this.drawer,
            scores: this.getScores()
        };
    }
}