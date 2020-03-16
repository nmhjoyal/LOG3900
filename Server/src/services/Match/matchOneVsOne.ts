import Match from "./match"

export default class OneVsOne extends Match{

    public constructor(host: string, nbRounds: number) {
        super(host, nbRounds);
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