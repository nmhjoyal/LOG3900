import Player from "./player";

export interface Stats {
    
}
export interface StatsDB {
    username: string
    bestSSS: number // best Sprint solo score
    connections: Connection[]
    matchHistory: MatchHistory[]
}

export interface Connection {
    connect: number
    disconnect: number
}

export interface MatchHistory {
    time: Connection
    players: Player[]
}

// - statistiques d’utilisation du jeu 
    // * nombre de parties jouées : matchHistory.length
    // * pourcentage de victoires : matchHistory results look for winner.
    // * temps moyen d’une partie : matchHistory date and time
    // * temps total passé à jouer: matchHistory total time 
// - Best sprint solo score
// - Dates et heures connexion/deconnexion
// - Historique des parties jouées 
    // * date, heure
    // * nom des joueurs
    // * résultat pour chaque partie