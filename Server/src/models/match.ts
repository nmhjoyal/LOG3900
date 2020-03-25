import { MatchMode } from "./matchMode";
import PublicProfile from "./publicProfile";

export interface CreateMatch {
    nbRounds: number
    matchMode: MatchMode
}

export interface MatchInfos {
    matchId: string
    host: string
    nbRounds: number
    matchMode: MatchMode
    players: PublicProfile[] /* username, avatar */
}

export interface StartMatch {
    matchId: string
    timeLimit: number /* in seconds */
}

export const TIME_LIMIT_MIN: number = 30; /* 30 sec minimum */
export const TIME_LIMIT_MAX: number = 120;/* 2 min maximum */

export interface StartTurn {
    currentRound: number
}

export interface EndTurn {
    scores: Map<string, number> /* username, score */
    choices: string[]
    drawer: string  // indicates if he is the drawer in FreeForAll and OneVsOne.
                    // not used in SprintCoop and SprintSolo because the players are always guessing
}
