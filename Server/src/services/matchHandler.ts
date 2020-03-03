import Match from "./Match/match"
import { MatchMode, MatchInstance } from "../models/matchMode";

export default class MatchHandler {
    private currentMatches: Match[];

    public constructor() {
        this.currentMatches = new Array<Match>();
    }

    public startMatch(matchMode: MatchMode) {
        this.currentMatches.push(MatchInstance.getMatchClassInstance(matchMode));
    }
}