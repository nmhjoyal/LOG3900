import { MatchMode } from "./matchMode";
import PublicProfile from "./publicProfile";

export interface CreateMatch {
    nbRounds: number
    timeLimit: number /* in seconds */
    matchMode: MatchMode
}
export const NB_ROUNDS_MIN: number = 1; /* 30 sec minimum */
export const NB_ROUNDS_MAX: number = 10;/* 2 min maximum */
export const TIME_LIMIT_MIN: number = 30; /* 30 sec minimum */
export const TIME_LIMIT_MAX: number = 120;/* 2 min maximum */

export interface MatchInfos {
    matchId: string
    host: string
    nbRounds: number
    timeLimit: number /* in seconds */
    matchMode: MatchMode
    players: PublicProfile[] /* username, avatar */
}

export interface EndTurn {
    currentRound: number
    scores: UsernameUpdateScore[]/* username, score */
    choices: string[]
    drawer: string  // indicates if he is the drawer in FreeForAll and OneVsOne.
                    // not used in SprintCoop and SprintSolo because the players are always guessing
}
export interface UpdateScore {
    scoreTotal: number
    scoreTurn: number
}

export interface UsernameUpdateScore {
    username: string,
    scoreTotal: number
    scoreTurn: number
}
