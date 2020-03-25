import Match from "../services/Match/matchAbstract";
import FreeForAll from "../services/Match/matchFreeForAll";
import SprintSolo from "../services/Match/matchSprintSolo";
import SprintCoop from "../services/Match/matchSprintCoop";
import OneVsOne from "../services/Match/matchOneVsOne";
import Inverted from "../services/Match/matchInverted";
import { CreateMatch } from "./match";
import PublicProfile from "./publicProfile";
import ChatHandler from "../services/chatHandler";

export enum MatchMode {
    freeForAll = 1,
    sprintSolo = 2,
    sprintCoop = 3,
    oneVsOne = 4,
    inverted = 5
}

export class MatchInstance {
    public static createMatch(matchId: string, socketId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler): Match {
        switch (createMatch.matchMode) {
            case MatchMode.freeForAll:
                return new FreeForAll(matchId, socketId, user, createMatch.nbRounds, chatHandler);
            case MatchMode.sprintSolo:
                return new SprintSolo(matchId, socketId, user, createMatch.nbRounds, chatHandler);
            case MatchMode.sprintCoop:
                return new SprintCoop(matchId, socketId, user, createMatch.nbRounds, chatHandler);
            case MatchMode.oneVsOne:
                return new OneVsOne(matchId, socketId, user, createMatch.nbRounds, chatHandler);
            case MatchMode.inverted:
                return new Inverted(matchId, socketId, user, createMatch.nbRounds, chatHandler);
        }
    }
}