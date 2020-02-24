import { MongoClient} from "mongodb"
import PrivateProfile from "../../models/privateProfile";
import PublicProfile from "../../models/publicProfile"

const CONNECTION_URL: string = "mongodb+srv://Admin:HeB6OZmfIA6n9pfu@projet3db-jehvq.mongodb.net/test?retryWrites=true&w=majority"; 

class ProfileDB {
    private db: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(CONNECTION_URL, 
            {useUnifiedTopology: true, useNewUrlParser: true});

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            console.log("Connected to database.")
            this.db = db;
        });
    }

    public async createProfile(profile: PrivateProfile): Promise<void> {
        await this.db.db("Profiles").collection("profiles").insertOne(profile).catch((err: any) => {
            throw err;
        });
    }

    public async getPublicProfile(username: string): Promise<PublicProfile> {
        const publicProfile: PublicProfile = {
            username : "testreturn",
            avatar : "testavatar"
        }
        return publicProfile;
    }

    public async getPrivateProfile(username: string): Promise<PrivateProfile> {
        const privateProfile: PrivateProfile = {
            firstname : "string",
            lastname : "string",
            username : "string",
            password : "string",
            avatar : "string"/*String for the moment eventually needs to be image*/
        }
        return privateProfile;
    }

    public async deleteProfile(username: string): Promise<void> {
        await this.db.db("Profiles").collection("profiles").deleteOne({ username: username }).catch((err: any) => {
            throw err;
        });
    }
}

export var profileDB: ProfileDB = new ProfileDB();