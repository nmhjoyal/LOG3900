import { MatchMode } from "./matchMode";

export interface CreateMatch {
    nbRounds: number
    matchMode: MatchMode
}

export interface MatchInfos {
    host: string
    nbRounds: number
    matchMode: MatchMode
    players: Map<string, string> /* username, avatar */
}