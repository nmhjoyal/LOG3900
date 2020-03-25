import { Feedback, StartMatchFeedback, JoinRoomFeedback } from "../../models/feedback";
import { MatchInfos, StartMatch, TIME_LIMIT_MIN, TIME_LIMIT_MAX, UpdateScore } from "../../models/match";
import Player from "../../models/player";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";
import ChatHandler from "../chatHandler";
import PrivateProfile from "../../models/privateProfile";

export default abstract class Match {
    public matchId: string;
    public players: Map<string, Player>; /* socketid, Player */
    protected nbRounds: number;
    protected timeLimit: number;
    protected currentWord: string;
    protected isStarted: boolean;
    protected chatHandler: ChatHandler;

    // Depends on the instance
    protected mode: number;
    protected maxNbVP: number;

    protected timeout: NodeJS.Timeout; // setTimeout will be used for emitting end_round and we will cancel it if there is an unexpected leave of a room
    protected scores: Map<string, UpdateScore> // Key: username, Value: score
    protected round: number;
    protected currentPlayers: IterableIterator<Player>; // In one round each player will draw one time.

    protected constructor(matchId: string, socketId: string, user: PublicProfile, nbRounds: number, chatHandler: ChatHandler) {
        this.players = new Map<string, Player>();
        this.players.set(socketId, this.createPlayer(user, true, false));
        this.isStarted = false;
        this.nbRounds = nbRounds;
        this.matchId = matchId;
        this.chatHandler = chatHandler;
    }

    /**
     * 
     * Functions used trough all match modes.
     * 
     */
    public async joinMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile): Promise<JoinRoomFeedback> {
        let joinRoomFeedback: JoinRoomFeedback = { feedback: { status: true, log_message: "" }, room_joined: null, isPrivate: true };

        if (!this.isStarted) {
            joinRoomFeedback = await this.chatHandler.joinChatRoom(io, socket, this.matchId, user);
            this.players.set(socket.id, this.createPlayer(user, false, false));
            joinRoomFeedback.feedback.log_message = "You joined the match.";
        } else {
            joinRoomFeedback.feedback.status = false;
            joinRoomFeedback.feedback.log_message = "The match is already started.";
        }

        return joinRoomFeedback;
    }

    // boolean : true -> delete match
    public async leaveMatch(io: SocketIO.Server, socket: SocketIO.Socket, user: PrivateProfile): Promise<boolean> {
        const player: Player | undefined = this.players.get(socket.id);
        let deleteMatch: boolean = false;

        // TODO: some other checks for mode restriction :
        //      - this.players.length < min player for match mode, or maybe he can be replaced by virtual player?
        if (player) {
            await this.chatHandler.leaveChatRoom(io, socket, this.matchId, user);
            if (this.players.size - this.getNbVirtualPlayers() > 1) {
                if (this.isStarted) {
                    // Not important to check if he's the host here.
                    this.players.delete(socket.id);
                    if (player.isCurrent && !player.isVirtual) {
                        // If in between endRound and startRound (the player is choosing a word) 
                        // maybe feedback false log_
                        // sinon this.endRound(); et gérer 
                    }
                } else {
                    if (player.isHost) {
                        // Choose random new host in the players and notify him to change 
                        // his interface so he can start selecting the match settings.
                    } else {
                        this.players.delete(socket.id);
                    }
                }

            } else {
                deleteMatch = true;
            }
        }

        return deleteMatch;
    }

    public addVirtualPlayer(socketId: string, io: SocketIO.Server): Feedback {
        const player: Player | undefined = this.players.get(socketId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (player) {
            if (player.isHost) {
                const nbVirtualPlayers: number = this.getNbVirtualPlayers();
                if (nbVirtualPlayers < this.maxNbVP) {
                    /* EVENTUALLY, GENERATE RANDOM VP, also need to check if it is already in the players array (so we dont have two identical VP)*/ 
                    const randomVP: PublicProfile = { username: "Mr Avocado", avatar: "AVOCADO" };
                    this.players.set("VP" + nbVirtualPlayers, this.createPlayer(randomVP, false, true));
                    this.chatHandler.findPrivateRoom(this.matchId)?.avatars.set(randomVP.username, randomVP.avatar);
                    this.chatHandler.notifyAvatarUpdate(io, randomVP, this.matchId);
                    io.in(this.matchId).emit("update_players", JSON.stringify(this.getPlayersPublicProfile()));
                    feedback.status = true;
                    feedback.log_message = "A virtual player was added.";
                } else {
                    feedback.log_message = "You can not have more than" + this.maxNbVP + "virtual players in this mode.";
                }
            } else {
                feedback.log_message = "You are not the host. Only the host can add a virtual player.";
            }
        } else {
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback;
    }

    public removeVirtualPlayer(socketId: string, io: SocketIO.Server): Feedback {
        const player: Player | undefined = this.players.get(socketId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (player) {
            if (player.isHost) {
                const nbVirtualPlayers: number = this.getNbVirtualPlayers();
                if (nbVirtualPlayers > 0) {
                    this.players.delete("VP" + nbVirtualPlayers);
                    io.in(this.matchId).emit("update_players", JSON.stringify(this.getPlayersPublicProfile()));
                    feedback.status = true;
                    feedback.log_message = "A virtual player was removed."
                } else {
                    feedback.log_message = "There is already no virtual player."
                }
            } else {
                feedback.log_message = "You are not the host. Only the host can remove a virtual player.";
            }
        } else {
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback;
    }

    /**
     * 
     * From the moment start match is called the server takes over the conversation with the players.
     * The match routine starts. The routine varies depending on the match mode.
     *
     */
    public startMatch(socketId: string, io: SocketIO.Server, startMatch: StartMatch): StartMatchFeedback {
        const player: Player | undefined = this.players.get(socketId);
        let startMatchFeedback: StartMatchFeedback = { feedback : { status: false, log_message: "" }, nbRounds : 0 };

        if (player) {
            if (player.isHost) {
                if (startMatch.timeLimit > TIME_LIMIT_MIN && startMatch.timeLimit < TIME_LIMIT_MAX) {
                    this.isStarted = true;
                    this.timeLimit = startMatch.timeLimit;
                    this.currentPlayers = this.players.values();
                    this.round = 1;
                    this.initScores();
                    this.endTurn(io);
                    startMatchFeedback.nbRounds = this.nbRounds;
                    startMatchFeedback.feedback.status = true;
                    startMatchFeedback.feedback.log_message = "Match is going to start...";
                } else {
                    startMatchFeedback.feedback.log_message = "Time limit has to be in between 30 seconds and 2 minutes.";
                }
            } else {
                startMatchFeedback.feedback.log_message = "You are not the host. Only the host can start the match.";
            }
        } else {
            startMatchFeedback.feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return startMatchFeedback;
    }
    
    protected abstract startTurn(io: SocketIO.Server, chosenWord: string, isVirtual: boolean): void;
    protected abstract endTurn(io: SocketIO.Server): void;
    
    protected endMatch(): void {

    }

    protected initScores(): void {
        this.scores = new Map<string, UpdateScore>();
        this.players.forEach((player: Player) => {
            if(!player.isVirtual) {
                this.scores.set(player.user.username, { scoreTotal: 0, scoreTurn: 0 });
            }
        });
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

        this.players.forEach((player: Player) => {
            if (player.isVirtual) count++;
        });

        return count;
    }

    public getPlayersPublicProfile(): PublicProfile[] {
        let publicProfiles: PublicProfile[] = [];
        
        this.players.forEach((player: Player) => {
            publicProfiles.push(player.user);
        });

        return publicProfiles;
    }

    public getMatchInfos(): MatchInfos | undefined {
        let matchInfos: MatchInfos | undefined;
        
        if (!this.isStarted && this.mode !== MatchMode.sprintSolo) { // maybe this.players.length < this.maxPlayers?    
            let userInfos: PublicProfile[] = [];
            let host: string = "";

            this.players.forEach((player: Player) => {
                if (player.isHost) host = player.user.username; 
                userInfos.push(player.user);
            });

            matchInfos = {
                matchId: this.matchId,
                host: host,
                matchMode: this.mode,
                nbRounds: this.nbRounds,
                players: userInfos
            };
        }

        return matchInfos;
    }

    protected createPlayer(user: PublicProfile, isHost: boolean, isVirtual: boolean): Player {
        return {
            user: user,
            isHost: isHost,
            isVirtual: isVirtual,
            isCurrent: false,
            score: 0
        };
    }

}