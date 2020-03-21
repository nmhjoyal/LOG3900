import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";

export default class FreeForAll extends Match {

    public constructor(matchId: string, host: string, user: PublicProfile, nbRounds: number) {
        super(matchId, host, user, nbRounds);
        this.mode = 1;
        this.maxNbVP = 4;
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