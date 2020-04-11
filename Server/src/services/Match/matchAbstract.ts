import { Feedback, StartMatchFeedback, JoinRoomFeedback } from "../../models/feedback";
import { MatchInfos, CreateMatch, EndTurn, StartTurn, UpdateSprint } from "../../models/match";
import Player, { UpdateScore } from "../../models/player";
import PublicProfile from "../../models/publicProfile";
import { MatchMode, MatchSettings } from "../../models/matchMode";
import ChatHandler from "../chatHandler";
import PrivateProfile from "../../models/privateProfile";
import { VirtualDrawing } from "../Drawing/virtualDrawing";
import { Drawing } from "../Drawing/drawing";
import { Stroke, StylusPoint, Level } from "../../models/drawPoint";
import RandomWordGenerator from "../WordGenerator/wordGenerator";
import Admin from "../../models/admin";
import { Message } from "../../models/message";
import VirtualPlayer from "../VirtualPlayer/virtualPlayer";
import { rankingDB } from "../Database/rankingDB";
import { statsDB } from "../Database/statsDB";

export default abstract class Match {
    // Settings
    public matchId: string;
    public isEnded: boolean;
    public players: Player[]; /* socketid, Player */
    public mode: number;
    protected nbRounds: number;
    protected timeLimit: number;
    protected hints: string[];
    protected currentWord: string;
    protected isStarted: boolean;
    protected chatHandler: ChatHandler;
    protected startTime: number;

    // Depends on the instance
    protected readonly ms: MatchSettings;

    // During the match
    protected timer: number;
    protected timeouts: NodeJS.Timeout[];         // setTimeout will be used for emitting end_turn and we will cancel it 
                                                // if there is an unexpected leave of a room or stoppage of a turn
    protected round: number;                    // In one round each player will draw one time
    protected drawer: Player;                   // Username 
    protected drawing: Drawing;
    protected virtualDrawing: VirtualDrawing;
    protected virtualPlayer: VirtualPlayer;
    protected vp: string;
    protected gameLevel: Level;
    protected guessCounter: number;

    // Match methods
    public async abstract startTurn(io: SocketIO.Server, chosenWord: string): Promise<void>;
    public async abstract endTurn(io: SocketIO.Server): Promise<void>;
    public abstract guessRight(io: SocketIO.Server, username: string): void;

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
        this.vp = "";
        this.currentWord = "";
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
                    this.players.push(this.createPlayer(this.getPublicProfile(user)));
                    io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
                    joinRoomFeedback.feedback.log_message = "You joined the match.";
                } else {
                    joinRoomFeedback.feedback.log_message = "You can not have more than " + this.ms.MAX_NB_HP + " human players in this mode.";
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
                if (this.getNbHumanPlayers() > this.ms.MIN_NB_HP) {
                    if (player == this.drawer) {
                        this.endTurn(io);
                    }
                } else {
                    console.log("unexpected_leave")
                    io.in(this.matchId).emit("unexpected_leave");
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
                        feedback.log_message = "You can not have more than " + this.ms.MAX_NB_VP + " virtual players in this mode.";
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
                    feedback.log_message = "You can not have less than " + this.ms.MIN_NB_VP + " virtual players in this mode."
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
                if (nbHumanPlayers >= this.ms.MIN_NB_HP && nbHumanPlayers <= this.ms.MAX_NB_HP) {
                    const nbVirtualPlayers: number = this.getNbVirtualPlayers();
                    if (nbVirtualPlayers >= this.ms.MIN_NB_VP && nbVirtualPlayers <= this.ms.MAX_NB_VP) {
                        this.initMatch(io);
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

    public guess(io: SocketIO.Server, socket: SocketIO.Socket, guess: string, username: string): void {
        let feedback: Feedback = { status: false, log_message: "" };
        const drawerUsername: string = this.drawer.user.username;

        if (this.currentWord != "") {
            if (guess != "") {
                if (username != drawerUsername) {
                    if(guess.toUpperCase() == this.currentWord.toUpperCase()) {
                        // Depends on the instance
                        this.guessRight(io, username);
                        feedback.status = true;
                    } else {
                        this.decrementGuessCounter(io);
                        feedback.log_message = "Your guess is wrong.";
                    }
                } else {
                    feedback.log_message = "The player drawing is not supposed to guess.";
                }
            } else  {
                feedback.log_message = "The word guessed is empty.";
            }
        } else {
            feedback.log_message = "You can not guess when the round is not started.";
        }
   
        (this.mode == MatchMode.sprintCoop) ? 
            io.in(this.matchId).emit("guess_res", JSON.stringify(feedback)) :
            socket.emit("guess_res", JSON.stringify(feedback));
    }

    public hint(io: SocketIO.Server) {
        if (this.drawer.isVirtual) {
            io.in(this.matchId).emit("hint_disable");

            if (this.hints.length > 0) {
                const hint: string = this.hints.splice(Math.floor(Math.random() * (Math.floor(this.hints.length))), 1)[0];
                io.in(this.matchId).emit("new_message", this.virtualPlayer.getHintMessage(this.vp, hint, this.matchId));

                if (this.hints.length > 0) {
                    this.timeouts.push(setTimeout(() => {
                        io.in(this.matchId).emit("hint_enable");
                    }, 3000));
                }
            }
        }
    }

    protected async endMatch(io: SocketIO.Server): Promise<void> {
        // compile game stats for the players and the standings.
        await rankingDB.updateRanks(this.players, this.mode);
        await statsDB.updateMatchStats(this.players, this.mode, this.startTime);

        this.notifyWord(io);
        // notify everyone that the game is ended.
        io.in(this.matchId).emit("match_ended", this.players);
        
        this.isEnded = true; // to delete on future "update_matches" event called
    }
    
    protected notifyWord(io: SocketIO.Server): void {
        const message: Message = Admin.createAdminMessage("The word was " + this.currentWord, this.matchId);
        io.in(this.matchId).emit("new_message", JSON.stringify(message));
        io.in(this.matchId).emit("new_message", JSON.stringify(this.virtualPlayer.getEndTurnMessage(this.vp, this.matchId)));
    }

    protected reset(io: SocketIO.Server): void {
        this.timeouts.forEach(clearTimeout);
        this.virtualDrawing.clear(io);
        this.drawing.reset(io);
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
        for (let player of this.players) {
            player.score = {
                scoreTotal: 0,
                scoreTurn: 0
            };
        }
    }

    protected initMatch(io: SocketIO.Server): void {
        this.timeouts = [];
        this.hints = [];
        this.startTime = Date.now();
        this.isStarted = true;
        this.drawing = new Drawing(this.matchId);
        this.virtualDrawing = new VirtualDrawing(this.matchId, this.timeLimit);
        io.in(this.matchId).emit("new_message", JSON.stringify(this.virtualPlayer.getStartMatchMessage(this.vp, this.matchId)));

        // In the other modes the drawer is set to the virtual player in the constructor.
        if (this.mode == MatchMode.freeForAll) {
            // Init to the last player on round 0 so it resets in endTurn for round 1 with first player.
            this.drawer = this.players[this.players.length - 1];

            const username: string | undefined = this.getVPUsername(); 
            this.vp = (username) ? username : this.virtualPlayer.create().user.username;
        }
        this.round = 0;
        
        this.initScores();
    }

    protected resetScoresTurn(): void {
        for (let player of this.players) {
            player.score.scoreTurn = 0;
        }
    }

    protected decrementGuessCounter(io: SocketIO.Server): void {
        if (this.mode == MatchMode.sprintCoop || this.mode == MatchMode.sprintSolo) {
            this.guessCounter--;
            if (this.noMoreGuess())
                this.endTurn(io);
        }
    }

    protected noMoreGuess(): boolean {
        return this.guessCounter == 0;
    }

    protected matchIsEnded(): boolean {
        return this.round == this.nbRounds + 1;
    }

    protected everyoneHasGuessed(): boolean {
        let everyoneHasGuessed: boolean = true;

        for (let player of this.players) {
            if (player.score.scoreTurn == 0 && player != this.drawer && !player.isVirtual) 
                everyoneHasGuessed = false;
        }

        return everyoneHasGuessed;
    }

    protected updateTeamScore(score: number): void {
        for (let player of this.players) {
            if (!player.isVirtual) {
                const oldScore: number = player.score.scoreTotal;
                const updatedScore: UpdateScore = {
                    scoreTotal: oldScore + score,
                    scoreTurn: score
                };
                player.score = updatedScore;
            }
        }
    }

    protected updateScore(username: string, score: number): void {
        for (let player of this.players) {
            if (player.user.username == username) {
                const oldScore: number = player.score.scoreTotal;
                const updatedScore: UpdateScore = {
                    scoreTotal: oldScore + score,
                    scoreTurn: score
                };
                player.score = updatedScore;
            }
        }
    }

    protected timeLeft(): number {
        return Math.round(this.timeLimit - ((Date.now() - this.timer)/1000)); 
    }

    protected calculateScore(isSprint: boolean): number {
        return Math.round(this.timeLeft()) * 10 + ((isSprint) ? 0 : (1 - (this.getPlayerGuessCount()/this.getNbHumanPlayers())) * 100);
    }

    protected assignDrawer() {
        const currentIndex: number = this.players.indexOf(this.drawer);
        if (currentIndex == this.players.length - 1) {
            this.drawer = this.players[0];
            this.round++;
        } else {
            this.drawer = this.players[currentIndex + 1];
        }
    }

    protected assignHost(): void {
        // Place the new host at the beginning of the array.
        this.players.splice(0, 0, this.players.splice(this.findNewHostIndex(), 1)[0]);
    }

    protected findNewHostIndex(): number {
        return this.players.findIndex(player => !player.isVirtual);
    }

    protected addVP(io: SocketIO.Server): Player {
        const randomVP: Player = this.virtualPlayer.create();
        this.players.push(randomVP);
        this.chatHandler.findPrivateRoom(this.matchId)?.avatars.set(randomVP.user.username, randomVP.user.avatar);
        this.chatHandler.notifyAvatarUpdate(io, randomVP.user, this.matchId);
        return randomVP;
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

    private getPublicProfile(privateProfile: PrivateProfile): PublicProfile {
        return { username: privateProfile.username, avatar: privateProfile.avatar };
    }

    protected getPlayerGuessCount(): number {
        let playerGuessCount: number = 0;

        for (let player of this.players) {
            if (player.score.scoreTurn != 0 && player != this.drawer) 
                playerGuessCount++;
        }

        return playerGuessCount;
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
        
        if (!this.isStarted && this.mode !== MatchMode.sprintSolo) {    
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

    protected getVPUsername(): string | undefined {
        return this.players.find(player => player.isVirtual)?.user.username;
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
            score: {
                scoreTotal: 0,
                scoreTurn: 0
            },
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
            const avatar: string = this.getAvatar(username);
            if(avatar) {}
            scores.push({
                username: username,
                avatar: this.getAvatar(username),
                updateScore: updateScore
            })
        });
        return scores;
    }

    protected createEndTurn(): EndTurn {
        return {
            currentRound: this.round,
            players: this.players,
            choices: RandomWordGenerator.generateChoices(),
            drawer: this.drawer.user.username
        };
    }

    protected createUpdateSprint(guess: number, word: string, time: number): UpdateSprint {
        return {
            players: this.players,
            guess: guess,
            word: word.replace(/[a-z]/gi, '_'),
            time: time
        }
    }
}