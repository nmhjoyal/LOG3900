import PublicProfile from "./publicProfile";

export default interface Player {
    user: PublicProfile
    score: UpdateScore
    isVirtual: boolean
}

export interface UpdateScore {
    scoreTotal: number
    scoreTurn: number
}