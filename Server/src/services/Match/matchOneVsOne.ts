import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";
import ChatHandler from "../chatHandler";

export default class OneVsOne extends Match {
    public guess(io: import("socket.io").Server, guess: string, username: string): void {
        throw new Error("Method not implemented.");
    }

    public constructor(matchId: string, user: PublicProfile, nbRounds: number, chatHandler: ChatHandler) {
        super(matchId, user, nbRounds, chatHandler);
        this.mode = MatchMode.oneVsOne;
        this.maxNbVP = 0;
    }

    public startTurn(io: SocketIO.Server, chosenWord: string, isVirtual: boolean): void {
        throw new Error("Method not implemented.");
    }

    public endTurn(io: SocketIO.Server): void {
        throw new Error("Method not implemented.");
    }
    
    public draw(): void {
        throw new Error("Method not implemented.");
    }
}