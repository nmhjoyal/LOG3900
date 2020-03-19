import Match from "./matchAbstract";
import { Feedback } from "../../models/feedback";

export default class SprintCoop extends Match {
    
    public constructor(host: string, nbRounds: number) {
        super(host, nbRounds);
        this.mode = 3;
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