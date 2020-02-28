import Match from "./Match/match"
import { GameMode, Game } from "../models/gameMode";

export default class MatchHandler {
    private currentMatches: Match[];

    public constructor() {
        this.currentMatches = new Array<Match>();
    }

    public startMatch(gameMode: GameMode) {
        this.currentMatches.push(Game.getMatchClassInstance(gameMode));
    }
}