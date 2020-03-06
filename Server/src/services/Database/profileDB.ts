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
        const privateProfileDB: any =  await this.mongoDB.db("Profiles").collection("profiles")
            .findOne({ username: { $eq: username } });
        
        let privateProfile: PrivateProfile | null = null;
        if(privateProfileDB) {
            // Otherwise _id (database id) is added to the object.
            privateProfile = {
                username : privateProfileDB.username,
                firstname : privateProfileDB.firstname,
                lastname : privateProfileDB.lastname,
                password : privateProfileDB.password,
                avatar : privateProfileDB.avatar,
                rooms_joined: privateProfileDB.rooms_joined
            }
        }
        
        return privateProfile;
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

    public async updateProfile(profile: PrivateProfile): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles")
            .replaceOne(
                {username: { $eq: profile.username}}, profile)
            .catch((err: any) => {
                throw err;
            });
    }

    public async deleteRoom(roomId: string): Promise<void> {
        await this.mongoDB.db("Profiles").collection("profiles")
            .updateMany(
                {},
                { $pull: { rooms_joined : roomId } }
            );
    }
}

export var profileDB: ProfileDB = new ProfileDB();