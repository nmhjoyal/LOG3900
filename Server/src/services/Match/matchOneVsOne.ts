import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch, EndTurn } from "../../models/match";
import { OneVsOneSettings } from "../../models/matchMode";
import { Game } from "../../models/drawPoint";
import { gameDB } from "../Database/gameDB";
import { Message } from "../../models/message";
import Admin from "../../models/admin";
import Player from "../../models/player";

export default class OneVsOne extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, io: SocketIO.Server) {
        super(matchId, user, createMatch, chatHandler, OneVsOneSettings);
        // Add the only virtual player in the mode 1vs1, sprint coop and solo
        const vp: Player = this.addVP(io);
        this.vp = vp.user.username;
        this.drawer = vp;
    }

    public async startTurn(io: SocketIO.Server, word: string): Promise<void> {
        this.currentWord = word;
        io.in(this.matchId).emit("turn_started", this.createStartTurn(this.currentWord));

        const game: Game = await gameDB.getGame(word);
        this.hints = game.clues;
        this.virtualDrawing.draw(io, game.drawing, game.level);
        
        this.timer = Date.now();
        this.timeout = setTimeout(() => {
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    public async endTurn(io: SocketIO.Server): Promise<void> {
        this.reset(io);

        this.round++;

        if (this.matchIsEnded()) {
            await this.endMatch(io);
        } else {
            const endTurn: EndTurn = this.createEndTurn();

            if (this.currentWord != "") { // currentWord is undefined at the first endTurn
                this.notifyWord(io);
            }

            io.in(this.matchId).emit("turn_ended", JSON.stringify(endTurn));
            
            this.resetScoresTurn();
            this.currentWord = "";
            
            if (this.drawer.isVirtual) {
                let word: string;
                setTimeout(() => {
                    this.startTurn(io, word);
                }, 5000);
                word = await gameDB.getRandomWord();
            }
            // else we wait for the drawer to send his choice of word in the "start_turn" event.
        }
    }

    public guessRight(io: SocketIO.Server, username: string): void {
        const message: Message = Admin.createAdminMessage(username + " guessed the word.", this.matchId);
        io.in(this.matchId).emit("new_message", JSON.stringify(message));

        const score: number = this.calculateScore(false);
        this.updateScore(username, score);

        io.in(this.matchId).emit("update_players", JSON.stringify(this.players));

        if(this.everyoneHasGuessed()) {
            this.endTurn(io);
        }
    }
}