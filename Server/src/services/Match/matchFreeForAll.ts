import Match from "./matchAbstract";
import { Feedback } from "../../models/feedback";
import { StartMatch, TIME_LIMIT_MIN, TIME_LIMIT_MAX } from "../../models/match";

export default class FreeForAll extends Match {

    public constructor(host: string, nbRounds: number) {
        super(host, nbRounds);
        this.mode = 1;
    }

    public startMatch(io: SocketIO.Server, startMatch: StartMatch): Feedback {
        let feedback: Feedback = { status: false, log_message: "" };
        const timeLimit = startMatch.timeLimit;
        const nbVP = startMatch.nbVirtualPlayer;

        if (timeLimit > TIME_LIMIT_MIN && timeLimit < TIME_LIMIT_MAX) {
            if (nbVP > 4) {
                this.addVirtualPlayers(nbVP) // add nbVP virtual players to the players map 
                this.letterReveal = startMatch.letterReveal;
                // TODO: match logic ... 
            } else {
                feedback.log_message = "You can not have more than 4 virtual players."
            }
        } else {
            feedback.log_message = "Time limit has to be in between 30 seconds and 2 minutes.";
        }

        return feedback
    }

    public startRound(): void {
        throw new Error("Method not implemented.");
    }
    
    public endRound(): void {
        throw new Error("Method not implemented.");
    }
    
    public draw(): void {
        throw new Error("Method not implemented.");
    }
}