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

export interface MatchSettings {
    MAX_NB_PLAYERS: number;
    MAX_NB_VP: number;
    MIN_NB_VP: number;
    MIN_NB_HP: number;
    MAX_NB_HP: number;
}

export const freeForAllSettings: MatchSettings = {
    MAX_NB_PLAYERS : 8, // A combination of maximum 8 human and virtual players.
    MIN_NB_VP : 0, 
    MAX_NB_VP : 7,      // in the case that there is 1 human player.
    MIN_NB_HP : 1,
    MAX_NB_HP : 8       // if there is no virtual players.
}
export const sprintSoloSettings: MatchSettings = {
    MAX_NB_PLAYERS : 2, // 1 human player + 1 virtual player.
    MIN_NB_VP : 1,  
    MAX_NB_VP : 1, 
    MIN_NB_HP : 1,
    MAX_NB_HP : 1 
}
export const sprintCoopSettings: MatchSettings = {
    MAX_NB_PLAYERS : 5, // 4 human players + 1 virtual player.
    MIN_NB_VP : 1, 
    MAX_NB_VP : 1,
    MIN_NB_HP : 2,      // otherwise it is a sprintSolo 
    MAX_NB_HP : 4
}
export const OneVsOneSettings: MatchSettings = {
    MAX_NB_PLAYERS : 3, // 2 human players + 1 virtual player.
    MIN_NB_VP : 1, 
    MAX_NB_VP : 1,
    MIN_NB_HP : 2,
    MAX_NB_HP : 2       // if there is no virtual players.
}

