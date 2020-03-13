import { Room } from "../../models/room";
import { Feedback } from "../../models/feedback";

export default class Match {
    protected players: string[]; /* Socket ids */
    protected nbRounds: number;
    protected letterReveal: boolean;
    // Chat associated to the game.
    protected matchChat: Room;
    protected isStarted: boolean;

    protected constructor(host: string, nbRounds: number) {
        this.players = [host];
        this.nbRounds = nbRounds;
        this.isStarted = false;
    }

    public joinMatch(socketId: string): Feedback {
        let feedback: Feedback = {
            status: true,
            log_message: "You joined the match."
        }

        if (!this.isStarted) {
            this.players.push(socketId);
        } else {
            feedback.status = false
            feedback.log_message = "The match is already started."
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

}