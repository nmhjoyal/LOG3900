import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { EndTurn, CreateMatch, UsernameUpdateScore, UpdateScore } from "../../models/match";
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
        this.drawing.reset(io);
        
        if (isVirtual) {
            const game: Game = await gameDB.getGame(word);
            this.currentWord = game.word;
            this.virtualDrawing.draw(io, game.drawing, game.level);
        }
        
        this.timer = Date.now();
        console.log("before timeout");
        this.timeout = setTimeout(() => {
            if(isVirtual) {
                this.virtualDrawing.clear(io);
            }
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    protected async endTurn(io: SocketIO.Server): Promise<void> {
        clearTimeout(this.timeout);
        let currentPlayer: Player | undefined = this.getPlayer(this.currentPlayer);
        if (currentPlayer) {
            const currentIndex: number = this.players.indexOf(currentPlayer);
            if (currentIndex == this.players.length - 1) {
                this.currentPlayer = this.players[0].user.username;
                this.round++;
            } else {
                this.currentPlayer = this.players[currentIndex + 1].user.username;
            }

            const scores: UsernameUpdateScore[] = [];
            this.scores.forEach((updateScore: UpdateScore, username: string) => {
                const usernameUpdateScore: UsernameUpdateScore = {
                    username: username,
                    scoreTotal: updateScore.scoreTotal,
                    scoreTurn: updateScore.scoreTurn,
                }
                scores.push(usernameUpdateScore);
            })
    
            const endTurn: EndTurn = {
                currentRound: this.round,
                choices: RandomWordGenerator.generateChoices(),
                drawer: currentPlayer.user.username,
                scores: scores
            };
            const message: Message = Admin.createAdminMessage("The word was " + this.currentWord, this.matchId);
            this.currentWord = "";
            io.in(this.matchId).emit("new_message", JSON.stringify(message));
            console.log("turn_ended emitted");
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
        console.log("guess : " + guess);
        console.log("current word : " + this.currentWord);
        console.log("current player : " + this.currentPlayer);
        console.log("username : " + username);
        if(guess == this.currentWord && this.currentPlayer != username && this.currentWord != "") {
            const score: number = Math.round((Date.now() - this.timer) / 1000) * 10;
            this.updateScore(username, score);
            this.updateScore(this.currentPlayer, Math.round(score / this.players.length));
            if(this.everyoneHasGuessed()) {
                console.log(guess);
                this.endTurn(io);
            }
        }
    }
}