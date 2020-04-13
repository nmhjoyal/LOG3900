import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import { Stats, MatchHistory, StatsClient } from "../../models/stats";
import { MatchMode } from "../../models/matchMode";
import Player from "../../models/player";

class StatsDB {
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            { useUnifiedTopology: true, useNewUrlParser: true });

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.mongoDB = db;
            // this.mongoDB.db("Stats").collection("stats").createIndex({ username: 1 }, { unique: true });
        });
    }

    public async createStats(username: string): Promise<void> {
        const stats: Stats = {
            username: username,
            bestSSS: 0,
            connections: [],
            disconnections: [],
            matchesHistory: []
        };
        await this.mongoDB.db("Stats").collection("stats")
            .insertOne(stats)
            .catch((err: any) => {
                throw err;
            });
    }
    
    public async updateMatchStats(players: Player[], matchMode: MatchMode, startTime: number): Promise<void> {
        if (matchMode == MatchMode.sprintSolo) {
            await this.mongoDB.db("Stats").collection("stats").updateOne(
                { username: players[0].user.username },
                { $max: { bestSSS: players[0].score.scoreTotal } }
            );
        }
        
        const winner: Player = players.reduce(
            function(winner, player) { 
                return (player.score.scoreTotal > winner.score.scoreTotal) ? player : winner
            }
        );

        let matchHistory: MatchHistory = {
            startTime: startTime,
            endTime: Date.now(),
            matchMode: matchMode,
            winner: { username: winner.user.username, score: winner.score.scoreTotal },
            myScore: 0,
            playerNames: players.filter(player => !player.isVirtual).map(player => player.user.username)
        }
        
        for(let player of players) {
            if (!player.isVirtual) {
                matchHistory.myScore = player.score.scoreTotal;
                await this.mongoDB.db("Stats").collection("stats").updateOne(
                    { username: player.user.username },
                    { $push: { matchesHistory : matchHistory } }
                );
            }
        }
    }

    public async updateConnectionStats(username: string): Promise<void> {
        const date: number = Date.now();
        await this.mongoDB.db("Stats").collection("stats").updateOne(
            { username: username },
            { $push: { connections : date } }
        );
    }

    public async updateDisconnectionStats(username: string): Promise<void> {
        const date: number = Date.now();
        await this.mongoDB.db("Stats").collection("stats").updateOne(
            { username: username },
            { $push: { disconnections : date } }
        );
    }

    public async getStats(username: string): Promise<StatsClient> {
        const statsDB: any = await this.mongoDB.db("Stats").collection("stats").findOne({ username: username });
        const matchCount: number = statsDB.matchesHistory.length;
        const victoryCount: number = statsDB.matchesHistory.map((matchHistory: MatchHistory) => 
            matchHistory.winner.username).filter((winner: string) => winner == username ).length;

        const totalTime: number = statsDB.matchesHistory.reduce((totalTime: number, matchHistory: MatchHistory) => 
            totalTime + matchHistory.endTime - matchHistory.startTime, 0);

        return {
            username: statsDB.username,
            matchCount: matchCount,
            victoryPerc: (matchCount == 0) ? 0 : Math.round(victoryCount / matchCount * 100),
            averageTime: (matchCount == 0) ? 0 : Math.round(totalTime / matchCount / 1000),
            totalTime: Math.round(totalTime / 1000),
            bestSSS: statsDB.bestSSS,
            connections: statsDB.connections,
            disconnections: statsDB.disconnections,
            matchesHistory: statsDB.matchesHistory
        }
    }

    public deleteStats(username: string): void {
        this.mongoDB.db("Stats").collection("stats")
                .deleteOne({ username: username });
    }
}

export var statsDB: StatsDB = new StatsDB();