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

    public async getPublicProfile(username: string): Promise<PublicProfile | null> {
        const privateProfile: PrivateProfile | null = await this.db.db("Profiles").collection("profiles")
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

        const privateProfile: PrivateProfile | null = await this.db.db("Profiles").collection("profiles")
            .findOne({username: { $eq: username}})

        return privateProfile;
        
    }

    public async deleteProfile(username: string): Promise<void> {
        await this.db.db("Profiles").collection("profiles").deleteOne({ username: username });
    }
}

export var profileDB: ProfileDB = new ProfileDB();