import RandomIdGenerator from "../IdGenerator/idGen"
import { Room } from "../../models/room";

export default class Match {
    protected matchId: string;
    protected players: string[];
    protected nbRounds: number;
    protected letterReveal: boolean;
    // Chat associated to the game.
    protected matchChat: Room;

    // 
    protected constructor(host: string, nbRounds: number) {
        this.matchId = RandomIdGenerator.generate();
        this.players = [host];
        this.nbRounds = nbRounds;
    }

    public startMatch(): void {

    }

    public endMatch(): void {

    }

    public startRound(): void {

    }

    public endRound(): void {

    }

    public draw(): void { 

    }

}