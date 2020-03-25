import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";
import ChatHandler from "../chatHandler";

export default class SprintSolo extends Match {

    public constructor(matchId: string, host: string, user: PublicProfile, nbRounds: number, chatHandler: ChatHandler) {
        super(matchId, host, user, nbRounds, chatHandler);
        // add virtual player 
        // io.in(this.matchId).emit("update_players", JSON.stringify(this.getPlayersPublicProfile()));
        this.mode = MatchMode.sprintSolo;
        this.maxNbVP = 0;
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