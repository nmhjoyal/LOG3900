import { Feedback } from "../../models/feedback";
import PublicProfile from "../../models/publicProfile";
import { MatchInfos } from "../../models/match";
import { serverHandler } from "../serverHandler";
import PrivateProfile from "../../models/privateProfile";
import Player from "../../models/player";
import { Room } from "../../models/room";

export default abstract class Match {
    protected mode: number;
    protected players: Map<string, Player>; /* socketid, score */
    protected nbRounds: number;
    protected letterReveal: boolean;
    protected currentWord: string;
    protected matchChat: Room;
    public isStarted: boolean;

    protected constructor(socket: SocketIO.Socket, randomId: string, nbRounds: number) {
        this.isStarted = false;
        this.nbRounds = nbRounds;
    }

    public joinMatch(socket: SocketIO.Socket, user: PublicProfile): Feedback {
        let feedback: Feedback = {
            status: true,
            log_message: ""
        };

        if (!this.isStarted) {
            let player: Player = {
                isVirtual: false,
                isCurrent: false,
                score: 0
            };
            this.players.set(socket.id, player);
            feedback.status = true;
            feedback.log_message = "You joined the match.";
        } else {
            feedback.status = false;
            feedback.log_message = "The match is already started.";
        }

        return feedback;
    }

    public startMatch(): void {
        this.isStarted = true;
    }

    public endMatch(): void {

    }

    public startRound(): void {

    }

    public endRound(): void {

    }

    public draw(): void { 

    }

    public getMatchInfos(): MatchInfos {

        let users: Map<string, string> = new Map<string, string>(); 
        this.players.forEach((score: Player, socketid: string) => {
            let userInfo: PrivateProfile | undefined = serverHandler.getUser(socketid);
            if (userInfo) users.set(userInfo.username, userInfo.avatar);
        });

        let matchInfos: MatchInfos = {
            matchMode: this.mode,
            nbRounds: this.nbRounds,
            players: users
        };

        return matchInfos;
    }

}