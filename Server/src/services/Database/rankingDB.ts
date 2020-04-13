import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import { Rank, RankClient } from "../../models/rank";
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
            // this.mongoDB.db("Ranks").collection("FreeForAll").createIndex({ username: 1 }, { unique: true });
            // this.mongoDB.db("Ranks").collection("SprintSolo").createIndex({ username: 1 }, { unique: true });
            // this.mongoDB.db("Ranks").collection("SprintCoop").createIndex({ username: 1 }, { unique: true });
            // this.mongoDB.db("Ranks").collection("OneVsOne").createIndex({ username: 1 }, { unique: true });
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

    public async getRanks(username: string, matchMode: MatchMode): Promise<RankClient[]> {
        // Top 10
        const ranksDB: any = await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(matchMode))
                                        .find().sort({ score: -1 }).toArray();
        
        const ranks: RankClient[] = [];
        for (let i: number = 0; i <  Math.min(ranksDB.length, 10); i++) {
            const rank: RankClient = { username: ranksDB[i].username, score: ranksDB[i].score, pos: i + 1 };
            ranks.push(rank);
        }
        
        // Rank of the requester.
        const rankDB: Rank | undefined = ranksDB.find((rank: Rank) => rank.username == username); 
        if (rankDB) {
            const rank: RankClient = { username: rankDB.username, score: rankDB.score, pos: ranksDB.indexOf(rankDB) + 1 };
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