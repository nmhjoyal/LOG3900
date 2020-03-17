import Match from "./match_General";

export default class Inverted extends Match {

    public constructor(socket: SocketIO.Socket, host: string, nbRounds: number) {
        super(socket, host, nbRounds);
        this.mode = 5;
    }
    
    public startMatch(): void {
        throw new Error("Method not implemented.");
    }
    public endMatch(): void {
        throw new Error("Method not implemented.");
    }
    public startRound(): void {
        throw new Error("Method not implemented.");
    }
    public endRound(): void {
        throw new Error("Method not implemented.");
    }
    public draw(): void {
        throw new Error("Method not implemented.");
    }
}