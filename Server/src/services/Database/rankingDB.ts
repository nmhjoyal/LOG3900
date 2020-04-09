import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import { Rank } from "../../models/rank";
import { MatchMode, MatchInstance } from "../../models/matchMode";
import Player from "../../models/player";

class RankingDB {
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            { useUnifiedTopology: true, useNewUrlParser: true });

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.mongoDB = db;
        });
    }

    public async createRank(username: string): Promise<void> {
        const rank: Rank = { username: username, score: 0 };
        for (let i: number = 0; i < Object.keys(MatchMode).length / 2; i++) {
            await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(i))
                .insertOne(rank)
                .catch((err: any) => {
                    throw err;
                });
        }
    }

    public async updateRanks(players: Player[], matchMode: MatchMode): Promise<void> {
        for (let player of players) {
            if (!player.isVirtual) {
                await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(matchMode)).updateOne(
                    { username: player.user.username },
                    { $inc: { score: player.score.scoreTotal } }
                );
            }
        }
    }

    public async getRanks(username: string, matchMode: MatchMode): Promise<Rank[]> {
        // Top 10
        const ranksDB: any = await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(matchMode))
                                        .find().sort({ score: -1 }).toArray();
        
        const ranks: Rank[] = [];
        for (let rankDB of ranksDB) {
            const rank: Rank = { username: rankDB.username, score: rankDB.score };
            ranks.push(rank);
        }
        
        // Rank of the requester.
        const rankDB: any = await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(matchMode))
        .findOne({ username: { $eq: username }});
        const rank: Rank = { username: rankDB.username, score: rankDB.score };
        if(rank) {
            ranks.push(rank);
        }
        return ranks;
    }

    public deleteRank(username: string): void {
        for (let i: number = 0; i < Object.keys(MatchMode).length / 2; i++) {
            this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(i))
                .deleteOne({ username: username });
        }
    }

}

export var rankingDB: RankingDB = new RankingDB();