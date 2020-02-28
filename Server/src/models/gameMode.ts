import Match from "../services/Match/match";
import FreeForAll from "../services/Match/matchFreeForAll";
import SprintSolo from "../services/Match/matchSprintSolo";
import SprintCoop from "../services/Match/matchSprintCoop";
import OneVsOne from "../services/Match/matchOneVsOne";
import Inverted from "../services/Match/matchInverted";

export enum GameMode {
    freeForAll = 1,
    sprintSolo = 2,
    sprintCoop = 3,
    oneVsOne = 4,
    inverted = 5
}

export class Game {
    public static getMatchClassInstance(gameMode: GameMode): Match {
        switch (gameMode) {
            case GameMode.freeForAll:
                return new FreeForAll();
            case GameMode.sprintSolo:
                return new SprintSolo();
            case GameMode.sprintCoop:
                return new SprintCoop();
            case GameMode.oneVsOne:
                return new OneVsOne();
            case GameMode.inverted:
                return new Inverted();
        }
    }
}