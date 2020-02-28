import { MongoClient } from "mongodb";
import PrivateProfile from "../../models/privateProfile";
import * as ServerConfig from "../../serverConfig.json";

class ProfileDB {
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            {useUnifiedTopology: true, useNewUrlParser: true});

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.mongoDB = db;
        });
    }

    public async createProfile(profile: PrivateProfile): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles")
            .insertOne(profile)
            .catch((err: any) => {
                throw err;
            });
    }

    public async getPrivateProfile(username: string): Promise<PrivateProfile | null> {
        return await this.mongoDB.db("Profiles").collection("profiles")
            .findOne({ username: { $eq: username } })
    }

    public async deleteProfile(username: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles")
            .deleteOne({ username: username });
    }

    public async joinRoom(username: string, roomId: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles")
            .updateOne(
                { username: username },
                { $push: { rooms_joined : roomId } }
            );
    }

    public async leaveRoom(username: string, roomId: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles")
            .updateOne(
                { username: username },
                { $pull: { rooms_joined : roomId } }
            );
    }

    public async updateProfile(profile: PrivateProfile): Promise<void>{
        await this.mongoDB.db("Profiles").collection("profiles")
            .replaceOne(
                {username: { $eq: profile.username}}, profile)
            .catch((err: any) => {
                throw err;
            });
    }
}

export var profileDB: ProfileDB = new ProfileDB();