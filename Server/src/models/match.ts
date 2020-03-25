import { MatchMode } from "./matchMode";
import PublicProfile from "./publicProfile";

export interface CreateMatch {
    nbRounds: number
    matchMode: MatchMode
}

export interface MatchInfos {
    host: string
    nbRounds: number
    matchMode: MatchMode
    players: PublicProfile[] /* username, avatar */
}

export interface StartMatch { 
    matchId: string
    letterReveal: boolean
    timeLimit: number /* in seconds */
    nbVirtualPlayer: number
} 
export const TIME_LIMIT_MIN: number = 30; /* 30 sec minimum */
export const TIME_LIMIT_MAX: number = 120;/* 2 min maximum */

export interface MatchStarted {
    choices: string[]
    letterRevel: boolean
    nbRounds: number
    isCurrent: boolean  // indicates if he is the drawer in FreeForAll and OneVsOne.
                        // indicates if he is the judge in Inverted.
}
