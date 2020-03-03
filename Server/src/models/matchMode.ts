import Match from "../services/Match/match";
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
    public static getMatchClassInstance(matchMode: MatchMode): Match {
        switch (matchMode) {
            case MatchMode.freeForAll:
                return new FreeForAll();
            case MatchMode.sprintSolo:
                return new SprintSolo();
            case MatchMode.sprintCoop:
                return new SprintCoop();
            case MatchMode.oneVsOne:
                return new OneVsOne();
            case MatchMode.inverted:
                return new Inverted();
        }
    }
}