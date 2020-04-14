import { MatchMode } from "./matchMode";
import PublicProfile from "./publicProfile";
import Player from "./player";
import { Level } from "./drawPoint";

export const TIME_LIMIT_MIN: number = 30;   // 30 sec minimum
export const TIME_LIMIT_MAX: number = 120;  // 2 min maximum
export class SPRINT {
    public static getBonusTime(lvl: Level): number {
        switch (lvl) {
            case Level.Easy:
                return 10;
            case Level.Medium:
                return 20;
            case Level.Hard:
                return 30;
        }
    }

    public static getNbGuesses(lvl: Level): number {
        switch (lvl) {
            case Level.Easy:
                return 7;
            case Level.Medium:
                return 5;
            case Level.Hard:
                return 3;
        }
    }
}

export interface CreateMatch {
    nbRounds: number
    timeLimit: number /* in seconds */
    matchMode: MatchMode
}

export interface MatchInfos {
    matchId: string
    host: string
    nbRounds: number
    timeLimit: number /* in seconds */
    matchMode: MatchMode
    players: PublicProfile[] /* username, avatar */
}

export interface StartTurn {
    timeLimit: number
    word: string
}

export interface EndTurn {
    currentRound: number
    players: Player[]
    choices: string[]
    drawer: string  // indicates who is the drawer useful in FreeForAll
}

export interface UpdateSprint {
    guess: number // number of guess
    time: number // time updated
    word: string // hidden word (with underscores)
    players: Player[]
}
