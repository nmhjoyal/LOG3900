import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch } from "../../models/match";
import { Feedback } from "../../models/feedback";
import { sprintCoopSettings } from "../../models/matchMode";

export default class SprintCoop extends Match {
    
    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, io: SocketIO.Server) {
        super(matchId, user, createMatch, chatHandler, sprintCoopSettings);
        // Add the only virtual player in the mode 1vs1, sprint coop and solo
        this.vp = this.addVP(io).user.username;
    }

    public async startTurn(io: SocketIO.Server, chosenWord: string): Promise<void> {
        throw new Error("Method not implemented.");
    }

    public async endTurn(io: SocketIO.Server): Promise<void> {
        throw new Error("Method not implemented.");
    }

    public guess(io: import("socket.io").Server, guess: string, username: string): Feedback {
        throw new Error("Method not implemented.");
    }
}