import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch,} from "../../models/match";
import { gameDB } from "../Database/gameDB";
import { Game } from "../../models/drawPoint";
import { Message } from "../../models/message";
import Admin from "../../models/admin";
import { freeForAllSettings } from "../../models/matchMode";

export default class FreeForAll extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler) {
        super(matchId, user, createMatch, chatHandler, freeForAllSettings);
    }

    public async startTurn(io: SocketIO.Server, word: string): Promise<void> {
        this.currentWord = word;
        io.in(this.matchId).emit("turn_started", this.createStartTurn(this.currentWord));
        
        if (this.drawer.isVirtual) {
            const game: Game = await gameDB.getGame(word);
            this.virtualDrawing.draw(io, game.drawing, game.level);
        }
        
        this.timer = Date.now();
        this.timeout = setTimeout(() => {
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    public async endTurn(io: SocketIO.Server): Promise<void> {
        this.reset(io);

        this.assignDrawer();

        if (this.matchIsEnded()) {
            this.endMatch(io);
        } else {
            this.endTurnGeneral(io);
        }
    }

    public guessRight(io: SocketIO.Server, username: string): void {
        const message: Message = Admin.createAdminMessage(username + " guessed the word.", this.matchId);
        io.in(this.matchId).emit("new_message", JSON.stringify(message));

        const score: number = Math.round((Date.now() - this.timer) / 1000) * 10;
        this.updateScore(username, score);
        if (!this.drawer.isVirtual) 
            this.updateScore(this.drawer.user.username, Math.round(score / this.players.length));

        io.in(this.matchId).emit("update_players", JSON.stringify(this.players));

        if(this.everyoneHasGuessed()) {
            this.endTurn(io);
        }
    }

    public guessWrong(io: SocketIO.Server, username: string): void {
        // nothing to do here no number of guesses in free for all.
    }
}