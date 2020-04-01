import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch } from "../../models/match";
import { Feedback } from "../../models/feedback";
import { sprintCoopSettings } from "../../models/matchMode";

export default class SprintCoop extends Match {
    
    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler) {
        super(matchId, user, createMatch, chatHandler, sprintCoopSettings);
        // add virtual player 
        // io.in(this.matchId).emit("update_players", JSON.stringify(this.getPlayersPublicProfile()));
    }

    public startTurn(io: SocketIO.Server, chosenWord: string, isVirtual: boolean): void {
        throw new Error("Method not implemented.");
    }

    public endTurn(io: SocketIO.Server): void {
        throw new Error("Method not implemented.");
    }

    public guess(io: import("socket.io").Server, guess: string, username: string): Feedback {
        throw new Error("Method not implemented.");
    }
}