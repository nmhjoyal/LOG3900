import { Feedback } from "../../models/feedback";
import { MatchInfos } from "../../models/match";
import PrivateProfile from "../../models/privateProfile";
import Player from "../../models/player";

export default abstract class Match {
    protected mode: number;
    protected players: Map<string, Player>; /* username, score */
    protected nbRounds: number;
    protected letterReveal: boolean;
    protected currentWord: string;
    public isStarted: boolean;

    protected constructor(username: string, nbRounds: number) {
        this.players = new Map<string, Player>();
        this.players.set(username, this.createPlayer(true, false));
        this.isStarted = false;
        this.nbRounds = nbRounds;
    }

    /**
     * 
     * From the moment start match is called the server takes over the conversation with the players.
     * The match routine starts. The routine varies depending on the match mode.
     *
     */
    public abstract startMatch(): Feedback;
    protected abstract endRound(): void;
    protected abstract startRound(): void;

    /**
     * 
     * Functions used trough all match modes.
     * 
     */
    public joinMatch(username: string): Feedback {
        let feedback: Feedback = {
            status: true,
            log_message: ""
        };

        if (!this.isStarted) {
            this.players.set(username, this.createPlayer(false, false));
            feedback.log_message = "You joined the match.";
        } else {
            feedback.status = false;
            feedback.log_message = "The match is already started.";
        }

        return feedback;
    }

    public leaveMatch(username: string): Feedback {
        const player: Player | undefined = this.players.get(username);
        let feedback: Feedback = {
            status: true,
            log_message: ""
        };
        
        if (player) {
            if (this.isStarted) {
                // Not important to check if he's the host here.
                this.players.delete(username);
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
                    this.players.delete(username);
                }
            }
            feedback.log_message = "You left the match.";
        } else {
            feedback.log_message = "This player is not in this match, unexpected error!";
        }

        return feedback;
    }

    // This method should be the same for every mode.
    protected endMatch(): void {

    }

    public getMatchInfos(users: Map<string, PrivateProfile>): MatchInfos {
        let userInfos: Map<string, string> = new Map<string, string>(); 
        this.players.forEach((score: Player, socketid: string) => {
            let userInfo: PrivateProfile | undefined = users.get(socketid);
            if (userInfo) userInfos.set(userInfo.username, userInfo.avatar);
        });

        let matchInfos: MatchInfos = {
            matchMode: this.mode,
            nbRounds: this.nbRounds,
            players: userInfos
        };

        return matchInfos;
    }

    protected createPlayer(isHost: boolean, isVirtual: boolean) {
        const host: Player = {
            isHost: isHost,
            isVirtual: isVirtual,
            isCurrent: false,
            score: 0
        };
        return host;
    }

}