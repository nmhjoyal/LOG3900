import Match from "./matchAbstract";
import { Feedback } from "../../models/feedback";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";

export default class SprintCoop extends Match {
    
    public constructor(matchId: string, host: string, user: PublicProfile, nbRounds: number) {
        super(matchId, host, user, nbRounds);
        this.mode = MatchMode.sprintCoop;
        this.maxNbVP = 0;
    }
    
    public startMatch(): Feedback {
        throw new Error("Method not implemented.");
    }
    public endMatch(): void {
        throw new Error("Method not implemented.");
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