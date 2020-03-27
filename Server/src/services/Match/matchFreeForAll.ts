import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { EndTurn, CreateMatch } from "../../models/match";
import RandomWordGenerator from "../wordGenerator/wordGenerator";
import Player from "../../models/player";
import { gameDB } from "../Database/gameDB";
import { Game } from "../../models/drawPoint";
import { Message } from "../../models/message";
import Admin from "../../models/admin";

export default class FreeForAll extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler) {
        super(matchId, user, createMatch, chatHandler);
        this.maxNbVP = 4;
    }

    public async startTurn(io: SocketIO.Server, word: string, isVirtual: boolean): Promise<void> {
        this.currentWord = word;
        io.in(this.matchId).emit("turn_started", this.timeLimit);
        
        if (isVirtual) {
            const game: Game = await gameDB.getGame(word);
            this.virtualDrawing.draw(io, game.drawing, game.level);
        }
        
        this.timer = Date.now();
        this.timeout = setTimeout(() => {
            if(isVirtual) {
                this.virtualDrawing.clear(io);
            }
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    protected async endTurn(io: SocketIO.Server): Promise<void> {
        clearTimeout(this.timeout);
        this.currentWord = "";
        let currentPlayer: Player | undefined = this.getPlayer(this.currentPlayer);
        if (currentPlayer) {
            const currentIndex: number = this.players.indexOf(currentPlayer);
            if (currentIndex == this.players.length - 1) {
                this.currentPlayer = this.players[0].user.username;
                this.round++;
            } else {
                this.currentPlayer = this.players[currentIndex + 1].user.username;
            }
    
            const endTurn: EndTurn = {
                currentRound: this.round,
                choices: RandomWordGenerator.generateChoices(),
                drawer: currentPlayer.user.username,
                scores: this.scores
            };
            const message: Message = Admin.createAdminMessage("The word was " + this.currentWord, this.matchId);
            io.in(this.matchId).emit("new_message", JSON.stringify(message));
            io.in(this.matchId).emit("turn_ended", JSON.stringify(endTurn));
            this.resetScoresTurn();
            
            if (currentPlayer.isVirtual) {
                let word: string;
                setTimeout(() => {
                    this.startTurn(io, word, true);
                }, 5000);
                word = await gameDB.getRandomWord();
            }
        }
    }

    public guess(io: SocketIO.Server, guess: string, username: string): void {
        if(guess == this.currentWord && this.currentPlayer != username && this.currentWord != "") {
            const score: number = Math.round((Date.now() - this.timer) / 1000) * 10;
            this.updateScore(username, score);
            this.updateScore(this.currentPlayer, Math.round(score / this.players.length));
            if(this.everyoneHasGuessed()) {
                this.endTurn(io);
            }
        }
    }
}