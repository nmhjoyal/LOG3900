

export default abstract class Match {
    // private matchId: number;
    // private players: string[];

    public constructor() { }

    // Eventuellement les methodes ne seront pas void, ils devront retourner le paquet a envoyer.
    public abstract startMatch(): void;
    public abstract endMatch(): void;
    public abstract startRound(): void;
    public abstract endRound(): void;
    public abstract draw(): void;

}