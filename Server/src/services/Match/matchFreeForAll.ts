import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";
import ChatHandler from "../chatHandler";

export default class FreeForAll extends Match {

    public constructor(matchId: string, host: string, user: PublicProfile, nbRounds: number, chatHandler: ChatHandler) {
        super(matchId, host, user, nbRounds, chatHandler);
        this.mode = MatchMode.freeForAll;
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