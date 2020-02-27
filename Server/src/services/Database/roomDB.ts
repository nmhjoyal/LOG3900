import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import Room from "../../models/room";
import Message from "../../models/message";

class RoomDB {
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

    public async createRoom(roomId: string): Promise<void> {
        console.log("roomId " + roomId);
        const room: Room = {
            name: roomId,
            messages: []
        };
        await this.mongoDB.db("Rooms").collection("rooms").insertOne(room).catch((err: any) => {
            throw err;
        });
    }

    public async addMessage(message: Message, roomId: string): Promise<void> {
        await this.mongoDB.db("Rooms").collection("rooms").updateOne(
            { name: roomId },
            { $push: { messages : message } }
        );
    }

    public async getRoom(roomId: string): Promise<Room | null> {
        const room: Room | null = await this.mongoDB.db("Rooms").collection("rooms")
            .findOne({name: { $eq: roomId}})

        return room;
    }
}

export var roomDB: RoomDB = new RoomDB();