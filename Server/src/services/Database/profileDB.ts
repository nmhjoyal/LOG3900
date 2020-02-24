import { MongoClient} from "mongodb"
import Profile from "../../models/profile";

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

    public async createProfile(profile: Profile): Promise<boolean> {
        return this.db.db("Profiles").collection("profiles").insertOne(profile, (err: any) => {
            // False if the username is already taken.
            return err.message.indexOf("11000"/*username already taken error code*/) != -1;
        });
        return true;
    }
}

export var profileDB: ProfileDB = new ProfileDB();