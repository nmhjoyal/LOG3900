import Match from "../services/Match/match_General";
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
    public static createMatch(socket: SocketIO.Socket, randomId: string, createMatch: CreateMatch): Match {
        switch (createMatch.matchMode) {
            case MatchMode.freeForAll:
                return new FreeForAll(socket, randomId, createMatch.nbRounds);
            case MatchMode.sprintSolo:
                return new SprintSolo(socket, randomId, createMatch.nbRounds);
            case MatchMode.sprintCoop:
                return new SprintCoop(socket, randomId, createMatch.nbRounds);
            case MatchMode.oneVsOne:
                return new OneVsOne(socket, randomId, createMatch.nbRounds);
            case MatchMode.inverted:
                return new Inverted(socket, randomId, createMatch.nbRounds);
        }
    }
}