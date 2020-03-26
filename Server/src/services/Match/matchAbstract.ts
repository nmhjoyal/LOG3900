import { Feedback } from "../../models/feedback";
import { MatchInfos, StartMatch, TIME_LIMIT_MIN, TIME_LIMIT_MAX } from "../../models/match";
import Player from "../../models/player";
import PublicProfile from "../../models/publicProfile";

export default abstract class Match {
    protected matchId: string;
    protected players: Map<string, Player>; /* socketid, Player */
    protected nbRounds: number;
    protected round: number;
    protected letterReveal: boolean;
    protected timeLimit: number;
    protected currentWord: string;
    protected nbVP: number;
    public isStarted: boolean;

    // Depends on the instance
    public mode: number;
    protected maxNbVP: number;

    protected setTimeoutId: number; // setTimeout will be used for emitting end_round and we will cancel it if there is an unexpected leave of a room
    protected playerTurn: number; // In one round each player will draw one time.

    protected constructor(matchId: string, socketId: string, user: PublicProfile, nbRounds: number) {
        this.players = new Map<string, Player>();
        this.players.set(socketId, this.createPlayer(user, true, false));
        this.isStarted = false;
        this.nbRounds = nbRounds;
        this.nbVP = 0;
        this.letterReveal = false;
        this.matchId = matchId;
    }

    /**
     * 
     * Functions used trough all match modes.
     * 
     */
    public joinMatch(socketId: string, user: PublicProfile): Feedback {
        let feedback: Feedback = { status: true, log_message: "" };

        if (!this.isStarted) {
            this.players.set(socketId, this.createPlayer(user, false, false));
            feedback.log_message = "You joined the match.";
        } else {
            feedback.status = false;
            feedback.log_message = "The match is already started.";
        }

        return feedback;
    }

    public leaveMatch(socketId: string): Feedback {
        const player: Player | undefined = this.players.get(socketId);
        let feedback: Feedback = { status: true, log_message: "" };

        // TODO: some other checks for mode restriction :
        //      - this.players.length < min player for match mode, or maybe he can be replaced by virtual player?
        if (player) {
            if (this.isStarted) {
                // Not important to check if he's the host here.
                this.players.delete(socketId);
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
                    this.players.delete(socketId);
                }
            }
            feedback.log_message = "You left the match.";
        } else {
            feedback.status = false;
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback;
    }

    public addVirtualPlayer(socketId: string, io: SocketIO.Server): Feedback {
        const player: Player | undefined = this.players.get(socketId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (player) {
            if (player.isHost) {
                if (this.nbVP < this.maxNbVP) {
                    /* EVENTUALLY, GENERATE RANDOM VP, also need to check if it is already in the players array (so we dont have two identical VP)*/ 
                    const randomVP: PublicProfile = { username: "random", avatar: "random" };
                    this.players.set("VP" + this.nbVP, this.createPlayer(randomVP, false, true));
                    this.nbVP++;
                    io.in(this.matchId).emit("update_players", JSON.stringify(this.getPlayersPublicProfile()));
                    feedback.status = true;
                    feedback.log_message = "A virtual player was added."
                } else {
                    feedback.log_message = "You can not have more than" + this.maxNbVP + "virtual players in this mode."
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
                if (this.nbVP > 0) {
                    this.players.delete("VP" + this.nbVP);
                    this.nbVP--;
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
    public startMatch(socketId: string, io: SocketIO.Server, startMatch: StartMatch): Feedback {
        const player: Player | undefined = this.players.get(socketId);
        let feedback: Feedback = { status: false, log_message: "" };

        if (player) {
            if (player.isHost) {
                if (startMatch.timeLimit > TIME_LIMIT_MIN && startMatch.timeLimit < TIME_LIMIT_MAX) {
                    this.letterReveal = startMatch.letterReveal;
                    this.timeLimit = startMatch.timeLimit;
                    // TODO: match logic ... 
                    feedback.status = true;
                    feedback.log_message = "Match is going to start...";
                } else {
                    feedback.log_message = "Time limit has to be in between 30 and 120 seconds.";
                }
            } else {
                feedback.log_message = "You are not the host. Only the host can start the match.";
            }
        } else {
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback
    }
    
    protected abstract startRound(): void;
    protected abstract endRound(): void;
    
    protected endMatch(): void {

    }

    public getPlayersPublicProfile(): PublicProfile[] {
        let publicProfiles: PublicProfile[] = [];
        this.players.forEach((player: Player) => {
            publicProfiles.push(player.user);
        });

        return publicProfiles;
    }

    public getMatchInfos(): MatchInfos {
        let userInfos: PublicProfile[] = [];
        let host: string = "";
        this.players.forEach((player: Player) => {
            if (player.isHost) host = player.user.username; 
            userInfos.push(player.user);
        });

        return {
            host: host,
            matchMode: this.mode,
            nbRounds: this.nbRounds,
            players: userInfos
        };
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