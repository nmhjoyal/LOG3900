import { MongoClient } from "mongodb";
import * as ServerConfig from "../../serverConfig.json";
import { Room } from "../../models/room";
import { Message } from "../../models/message";
import PublicProfile from "../../models/publicProfile.js";

class RoomDB {
    
    private mongoDB: MongoClient;

    public constructor() { 
        const mongoClient: MongoClient = new MongoClient(ServerConfig["connection-url"], 
            { useUnifiedTopology: true, useNewUrlParser: true });

        // Connect to database
        mongoClient.connect((err, db) => {
            if (err) throw err;
            this.mongoDB = db;
        });
    }

    public async createRoom(room: Room): Promise<void> {
        await this.mongoDB.db("Rooms").collection("rooms").insertOne(room).catch((err: any) => {
            throw err;
        });
    }

    public async addMessage(message: Message): Promise<void> {
        await this.mongoDB.db("Rooms").collection("rooms").updateOne(
            { id: message.roomId },
            { $push: { messages : message } }
        );
    }

    public async getRoom(roomId: string): Promise<Room | null> {
        const roomDB: any = await this.mongoDB.db("Rooms").collection("rooms")
            .findOne({id: { $eq: roomId}});

        let room: Room | null = null;
        if(roomDB) {
            room = {
                id: roomDB.id,
                messages: roomDB.messages,
                avatars: roomDB.avatars  
            }
        }
            
        return room;
    }

    public async deleteRoom(roomId: string): Promise<void> {
        await this.mongoDB.db("Rooms").collection("rooms")
            .deleteOne({ id: roomId });
        
    }

    public async mapAvatar(publicProfile: PublicProfile, roomId: string): Promise<void> {
        var qstr = `{ "$set": { "avatars.` + publicProfile.username + `" : "` + publicProfile.avatar + `"}}`;
        var query = JSON.parse(qstr);
        await this.mongoDB.db("Rooms").collection("rooms").updateOne(
                { id: roomId }, 
                query
            );
    }

    public async getRoomsByUser(username: string): Promise<string[]> {
        let rooms: string[] = [];
        await this.mongoDB.db("Rooms").collection("rooms").find(
            JSON.parse(`{ "avatars.` + username + `" : { "$exists": "true" } }`), 
            { projection: { id : 1 } }).forEach((room: Room) => {
                rooms.push(room.id);
            });
        return rooms;
    }
    
    public async getRooms(): Promise<string[]> {
        let rooms: string[] = [];
        await this.mongoDB.db("Rooms").collection("rooms").find(
            {}, { projection: { id : 1 } }).forEach((room: Room) => {
                rooms.push(room.id);
            });
        return rooms;
    }
}

export var roomDB: RoomDB = new RoomDB();