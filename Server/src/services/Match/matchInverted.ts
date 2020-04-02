import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch } from "../../models/match";
import { Feedback } from "../../models/feedback";
import { freeForAllSettings } from "../../models/matchMode";


/**
 * TO BE DELETED SOON NOT UPDATED.
 */

export default class Inverted extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler) {
        super(matchId, user, createMatch, chatHandler, freeForAllSettings); 
    }

    public startTurn(io: SocketIO.Server, socket: SocketIO.Socket, chosenWord: string, isVirtual: boolean): void {
        throw new Error("Method not implemented.");
    }

    public endTurn(io: SocketIO.Server): void {
        throw new Error("Method not implemented.");
    }

    public guess(io: SocketIO.Server, guess: string, username: string): Feedback {
        throw new Error("Method not implemented.");
    }
}