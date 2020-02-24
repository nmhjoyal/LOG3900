import { MongoClient} from "mongodb"
import PrivateProfile from "../../models/privateProfile";
import PublicProfile from "../../models/publicProfile"

const CONNECTION_URL: string = "mongodb+srv://Admin:HeB6OZmfIA6n9pfu@projet3db-jehvq.mongodb.net/test?retryWrites=true&w=majority"; 

export default class ProfileDB {
    private db: any;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(CONNECTION_URL, 
            {useUnifiedTopology: true, useNewUrlParser: true})

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.db = db;
        });
    }

    public async createProfile(profile: PrivateProfile): Promise<boolean> {
        return this.db.db("Profiles").collection("profiles").insertOne(profile, (err: any) => {
            // False if the username is already taken.
            return err.message.indexOf("11000"/*username already taken error code*/) != -1;
        });
        return true;
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
}

export var profileDB: ProfileDB = new ProfileDB();