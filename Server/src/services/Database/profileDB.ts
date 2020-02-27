import { MongoClient } from "mongodb";
import PrivateProfile from "../../models/privateProfile";
import PublicProfile from "../../models/publicProfile";
import * as ServerConfig from "../../serverConfig.json";

class ProfileDB {
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            {useUnifiedTopology: true, useNewUrlParser: true});

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            console.log("Connected to database.");
            // Laisser la ligne qui definit l'index?
            this.mongoDB = db;
        });
    }

    public async createProfile(profile: PrivateProfile): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles").insertOne(profile).catch((err: any) => {
            throw err;
        });
    }

    public async getPublicProfile(username: string): Promise<PublicProfile | null> {
        
        const privateProfile: PrivateProfile | null = await this.mongoDB.db("Profiles").collection("profiles")
            .findOne({username: { $eq: username}})

        if (privateProfile) {
            const publicProfile: PublicProfile = {
                username: privateProfile.username,
                avatar: privateProfile.avatar
            }
            return publicProfile;
        } else {
            return null;
        }
    }

    public async getPrivateProfile(username: string): Promise<PrivateProfile | null> {
        const privateProfile: PrivateProfile | null = await this.mongoDB.db("Profiles").collection("profiles")
            .findOne({ username: { $eq: username } })

        // console.log(privateProfile);
        return privateProfile;
        
    }

    public async deleteProfile(username: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles").deleteOne({ username: username });
    }

    public async joinRoom(username: string, roomId: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles").updateOne(
            { username: username },
            { $push: { rooms_joined : roomId } }
        );
    }

    public async leaveRoom(username: string, roomId: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles").updateOne(
            { username: username },
            { $pull: { rooms_joined : roomId } }
        );
    }
}

export var profileDB: ProfileDB = new ProfileDB();