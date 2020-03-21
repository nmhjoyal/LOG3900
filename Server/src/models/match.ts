import { MatchMode } from "./matchMode";

export interface CreateMatch {
    nbRounds: number
    matchMode: MatchMode
}

export interface StartMatch {
    matchId: string
    letterReveal: boolean
    timeLimit: number /* in seconds */
    nbVirtualPlayer: number
} 
export const TIME_LIMIT_MIN: number = 30; /* 30 sec minimum */
export const TIME_LIMIT_MAX: number = 120;/* 2 min maximum */

export interface MatchInfos {
    host: string
    nbRounds: number
    matchMode: MatchMode
    players: Map<string, string> /* username, avatar */
}
