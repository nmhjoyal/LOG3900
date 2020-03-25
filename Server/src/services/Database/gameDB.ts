import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import { Game } from "../../models/drawPoint";

class GameDB {
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            { useUnifiedTopology: true, useNewUrlParser: true });

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.mongoDB = db;
            this.mongoDB.db("Games").collection("games").createIndex({ word: 1 }, { unique: true });
        });
    }

    public async createGame(game: Game): Promise<void> {
        await this.mongoDB.db("Games").collection("games")
            .insertOne(game)
            .catch((err: any) => {
                throw err;
            });
    }

    public async getRandomGame(): Promise<Game> {
        const gameDB: any = (await this.mongoDB.db("Games").collection("games")
            .aggregate([ { $sample: { size: 1 } } ]).toArray())[0];
        const game: Game = {
            word: gameDB.word,
            drawing: gameDB.drawing,
            clues: gameDB.clues,
            level: gameDB.level,
        }
        return game;
    }
}

export var gameDB: GameDB = new GameDB();