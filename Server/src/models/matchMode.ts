import Match from "../services/Match/match_General";
import FreeForAll from "../services/Match/matchFreeForAll";
import SprintSolo from "../services/Match/matchSprintSolo";
import SprintCoop from "../services/Match/matchSprintCoop";
import OneVsOne from "../services/Match/matchOneVsOne";
import Inverted from "../services/Match/matchInverted";

export enum MatchMode {
    freeForAll = 1,
    sprintSolo = 2,
    sprintCoop = 3,
    oneVsOne = 4,
    inverted = 5
}

export class MatchInstance {
    public static createMatch(matchMode: MatchMode, host: string, nbRounds: number): Match {
        switch (matchMode) {
            case MatchMode.freeForAll:
                return new FreeForAll(host, nbRounds);
            case MatchMode.sprintSolo:
                return new SprintSolo(host, nbRounds);
            case MatchMode.sprintCoop:
                return new SprintCoop(host, nbRounds);
            case MatchMode.oneVsOne:
                return new OneVsOne(host, nbRounds);
            case MatchMode.inverted:
                return new Inverted(host, nbRounds);
        }
    }
}