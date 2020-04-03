import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch } from "../../models/match";
import { Feedback } from "../../models/feedback";
import { OneVsOneSettings } from "../../models/matchMode";

export default class OneVsOne extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, io: SocketIO.Server) {
        super(matchId, user, createMatch, chatHandler, OneVsOneSettings);
        // Add the only virtual player in the mode 1vs1, sprint coop and solo
        this.vp = this.addVP(io).user.username;

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