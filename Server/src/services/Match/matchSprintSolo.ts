import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch } from "../../models/match";
import { sprintSoloSettings } from "../../models/matchMode";
import { Message } from "../../models/message";
import Admin from "../../models/admin";
import { Game } from "../../models/drawPoint";
import { gameDB } from "../Database/gameDB";

export default class SprintSolo extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, io: SocketIO.Server) {
        super(matchId, user, createMatch, chatHandler, sprintSoloSettings);
        // Add the only virtual player in the mode 1vs1, sprint coop and solo
        this.vp = this.addVP(io).user.username;
    }

    public async startTurn(io: SocketIO.Server, word: string): Promise<void> {
        this.currentWord = word;
        io.in(this.matchId).emit("turn_started", this.createStartTurn(this.currentWord));

        const game: Game = await gameDB.getGame(word);
        this.virtualDrawing.draw(io, game.drawing, game.level);
        
        this.timer = Date.now();
        this.timeout = setTimeout(() => {
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    public async endTurn(io: SocketIO.Server): Promise<void> {
        this.reset(io);

        if (this.matchIsEnded()) {
            this.endMatch(io);
        } else {
            this.endTurnGeneral(io);
        }
    }

    public guessRight(io: SocketIO.Server, username: string): void {
        const message: Message = Admin.createAdminMessage(username + " guessed the word.", this.matchId);
        io.in(this.matchId).emit("new_message", JSON.stringify(message));

        // const score: number = Math.round((Date.now() - this.timer) / 1000) * 10;
        // this.updateScore(username, score);

        io.in(this.matchId).emit("update_players", JSON.stringify(this.players));

        this.endTurn(io);
    }

    public guessWrong(io: SocketIO.Server, username: string): void {
        // check for number of guesses left.
    }
}