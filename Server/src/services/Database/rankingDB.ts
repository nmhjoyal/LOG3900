import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import { Rank } from "../../models/rank";
import { MatchMode, MatchInstance } from "../../models/matchMode";

class RankingDB {
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            { useUnifiedTopology: true, useNewUrlParser: true });

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.mongoDB = db;
            // this.mongoDB.db("Ranks").collection("FreeForAll").createIndex({ score: -1 }, {});
            // this.mongoDB.db("Ranks").collection("SprintSolo").createIndex({ score: -1 }, {});
            // this.mongoDB.db("Ranks").collection("SprintCoop").createIndex({ score: -1 }, {});
            // this.mongoDB.db("Ranks").collection("OneVsOne").createIndex({ score: -1 }, {});
            // this.mongoDB.db("Ranks").collection("FreeForAll").createIndex({ username: 1 }, { unique: true });
            // this.mongoDB.db("Ranks").collection("SprintSolo").createIndex({ username: 1 }, { unique: true });
            // this.mongoDB.db("Ranks").collection("SprintCoop").createIndex({ username: 1 }, { unique: true });
            // this.mongoDB.db("Ranks").collection("OneVsOne").createIndex({ username: 1 }, { unique: true });
        });
    }

    public async addNewPlayerRank(username: string): Promise<void> {
        const rank: Rank = { username: username, score: 0 };
        for (let i: number = 0; i < Object.keys(MatchMode).length / 2; i++) {
            await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(i))
                .insertOne(rank)
                .catch((err: any) => {
                    throw err;
                });
        }
    }

    public async getRankings(username: string, matchMode: MatchMode): Promise<Rank[]> {
        const ranks: Rank[] = await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(matchMode))
                                        .find().limit(10).toArray();
        const rank: Rank | null = await this.mongoDB.db("Ranks").collection(MatchInstance.getModeName(matchMode))
                                        .findOne({ username: { $eq: username }});
        if(rank) {
            ranks.push(rank);
        }
        return ranks;
    }

}

export var rankingDB: RankingDB = new RankingDB();