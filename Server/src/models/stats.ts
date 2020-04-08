import { MatchMode } from "./matchMode";
import { Rank } from "./rank";

export interface StatsClient {
    username: string
    matchCount: number
    victoryPerc: number         // in %
    averageTime: number         // in seconds
    totalTime: number           // in seconds
    bestSSS: number
    connections: number[]
    disconnections: number[]
    matchesHistory: MatchHistory[]
}

export interface Stats {
    username: string
    bestSSS: number // best Sprint solo score
    connections: number[]
    disconnections: number[]
    matchesHistory: MatchHistory[]
}

export interface MatchHistory {
    startTime: number
    endTime: number
    matchMode: MatchMode
    playerNames: string[]
    winner: Rank
    myScore: number
}

// - statistiques d’utilisation du jeu 
    // * nombre de parties jouées : 
    // * pourcentage de victoires : 
    // * temps moyen d’une partie : 
    // * temps total passé à jouer:  
// - Best sprint solo score
// - Dates et heures connexion/deconnexion
// - Historique des parties jouées 
    // * date, heure (debutMatch, finMatch)
    // * nom des joueurs (usernames)
    // * résultat pour chaque partie (gagnant et son score, toi meme et ton score)