import { Room } from "../../models/room";

export default class Match {
    protected players: string[];
    protected nbRounds: number;
    protected letterReveal: boolean;
    // Chat associated to the game.
    protected matchChat: Room;
    protected isStarted: boolean;

    protected constructor(host: string, nbRounds: number) {
        this.players = [host];
        this.nbRounds = nbRounds;
        this.isStarted = false;
    }

    public joinMatch() {
        
    }

    public startMatch(): void {
        this.isStarted = true;
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