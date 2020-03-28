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
    freeForAll,
    sprintSolo,
    sprintCoop,
    oneVsOne,
    inverted
}

export class MatchInstance {
    public static createMatch(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler): Match {
        switch (createMatch.matchMode) {
            case MatchMode.freeForAll:
                return new FreeForAll(matchId, user, createMatch, chatHandler);
            case MatchMode.sprintSolo:
                return new SprintSolo(matchId, user, createMatch, chatHandler);
            case MatchMode.sprintCoop:
                return new SprintCoop(matchId, user, createMatch, chatHandler);
            case MatchMode.oneVsOne:
                return new OneVsOne(matchId, user, createMatch, chatHandler);
            case MatchMode.inverted:
                return new Inverted(matchId, user, createMatch, chatHandler);
        }
    }
}