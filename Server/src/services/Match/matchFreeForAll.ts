import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";
import ChatHandler from "../chatHandler";
import { EndTurn } from "../../models/match";
import RandomWordGenerator from "../wordGenerator/wordGenerator";
import Player from "../../models/player";
import { Game } from "../../models/drawPoint";
import { gameDB } from "../Database/gameDB";

export default class FreeForAll extends Match {

    public constructor(matchId: string, host: string, user: PublicProfile, nbRounds: number, chatHandler: ChatHandler) {
        super(matchId, host, user, nbRounds, chatHandler);
        this.mode = MatchMode.freeForAll;
        this.maxNbVP = 4;
    }

    public startTurn(io: SocketIO.Server, chosenWord: string, isVirtual: boolean): void {
        this.currentWord = chosenWord;
        io.in(this.matchId).emit("start_turn", this.timeLimit);

        this.timeout = setTimeout(() => {
            if (isVirtual) {
                // this.virtualDrawing.draw()
            }
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    public async endTurn(io: SocketIO.Server): Promise<void> {

        const currentPlayer: IteratorResult<Player, any> = this.currentPlayers.next();
        if(currentPlayer.done) {
            this.currentPlayers = this.players.values();
            this.round++;
        }

        const endTurn: EndTurn = {
            currentRound: this.round,
            choices: RandomWordGenerator.generate(3),
            drawer: currentPlayer.value.user.username,
            scores: this.scores
        };
        io.in(this.matchId).emit("end_turn", endTurn);

        if (currentPlayer.value.isVirtual) {
            let game: Game;
            setTimeout(() => {
                this.startTurn(io, game.word, true);
            }, 5000);
            game = await gameDB.getRandomGame();
        }
    }
}