import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";

class StatsDB {
    // private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            { useUnifiedTopology: true, useNewUrlParser: true });

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            // this.mongoDB = db;
        });
    }

    // public async createStats(profile: PrivateProfile): Promise<void> {
    //     await this.mongoDB.db("Profiles").collection("profiles")
    //         .insertOne(profile)
    //         .catch((err: any) => {
    //             throw err;
    //         });
    // }

    
}

export var statsDB: StatsDB = new StatsDB();