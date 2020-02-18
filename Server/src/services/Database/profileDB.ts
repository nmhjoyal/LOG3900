import { MongoClient } from "mongodb"

const CONNECTION_URL: string = "mongodb+srv://Hubert:<banane123>@projet3db-jehvq.mongodb.net/test?retryWrites=true&w=majority"; 

export default class ProfileDB {
    private db: any;

    public constructor() {
        MongoClient.connect(CONNECTION_URL, (err, db) => {
            if (err) throw err;
            this.db = db;
            console.log("connected");
        });
    }
}

export var profileDB: ProfileDB = new ProfileDB();