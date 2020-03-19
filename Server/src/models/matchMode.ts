import Match from "../services/Match/matchAbstract";
import FreeForAll from "../services/Match/matchFreeForAll";
import SprintSolo from "../services/Match/matchSprintSolo";
import SprintCoop from "../services/Match/matchSprintCoop";
import OneVsOne from "../services/Match/matchOneVsOne";
import Inverted from "../services/Match/matchInverted";
import { CreateMatch } from "./match";

export enum MatchMode {
    freeForAll = 1,
    sprintSolo = 2,
    sprintCoop = 3,
    oneVsOne = 4,
    inverted = 5
}

export class MatchInstance {
    public static createMatch(host: string, createMatch: CreateMatch): Match {
        switch (createMatch.matchMode) {
            case MatchMode.freeForAll:
                return new FreeForAll(host, createMatch.nbRounds);
            case MatchMode.sprintSolo:
                return new SprintSolo(host, createMatch.nbRounds);
            case MatchMode.sprintCoop:
                return new SprintCoop(host, createMatch.nbRounds);
            case MatchMode.oneVsOne:
                return new OneVsOne(host, createMatch.nbRounds);
            case MatchMode.inverted:
                return new Inverted(host, createMatch.nbRounds);
        }
    }
}