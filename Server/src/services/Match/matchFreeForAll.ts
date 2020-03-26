import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import { MatchMode } from "../../models/matchMode";
import ChatHandler from "../chatHandler";
import { EndTurn } from "../../models/match";
import RandomWordGenerator from "../wordGenerator/wordGenerator";
import Player from "../../models/player";
import { gameDB } from "../Database/gameDB";
import { Game } from "../../models/drawPoint";
import { Message } from "../../models/message";
import Admin from "../../models/admin";

export default class FreeForAll extends Match {

    public constructor(matchId: string, user: PublicProfile, nbRounds: number, chatHandler: ChatHandler) {
        super(matchId, user, nbRounds, chatHandler);
        this.mode = MatchMode.freeForAll;
        this.maxNbVP = 4;
    }

    public async startTurn(io: SocketIO.Server, word: string, isVirtual: boolean): Promise<void> {
        this.currentWord = word;
        io.in(this.matchId).emit("turn_started", this.timeLimit);
        
        if (isVirtual) {
            const game: Game = await gameDB.getGame(word);
            this.virtualDrawing.draw(io, game.drawing, game.level);
        }
        
        this.timeout = setTimeout(() => {
            if(isVirtual) {
                this.virtualDrawing.clear(io);
            }
            this.endTurn(io);
        }, this.timeLimit * 1000);
    }

    protected async endTurn(io: SocketIO.Server): Promise<void> {

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
            io.in(this.matchId).emit("new_message", message);
            io.in(this.matchId).emit("turn_ended", endTurn);
            if (currentPlayer.isVirtual) {
                let word: string;
                setTimeout(() => {
                    this.startTurn(io, word, true);
                }, 5000);
                word = await gameDB.getRandomWord();
            }
        }
    }
}