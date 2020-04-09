import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch, UpdateSprint, SPRINT } from "../../models/match";
import { sprintSoloSettings } from "../../models/matchMode";
import { Message } from "../../models/message";
import Admin from "../../models/admin";
import { Game } from "../../models/drawPoint";
import { gameDB } from "../Database/gameDB";
import Player from "../../models/player";

export default class SprintSolo extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler, io: SocketIO.Server) {
        super(matchId, user, createMatch, chatHandler, sprintSoloSettings);
        // Add the only virtual player in the mode 1vs1, sprint coop and solo
        const vp: Player = this.addVP(io);
        this.vp = vp.user.username;
        this.drawer = vp;
    }

    public async startTurn(io: SocketIO.Server, word: string): Promise<void> {
        this.currentWord = word;
        if (this.timer && this.gameLevel)
            // Calculate new timeLimit with bonus depending on last round difficulty and if all guesses were used
            this.timeLimit = this.timeLeft() + ((this.noMoreGuess()) ?  0 : SPRINT.getBonusTime(this.gameLevel));

        
        // Set up new game.
        const game: Game = await gameDB.getGame(word);
        this.hints = game.clues;
        this.gameLevel = game.level;
        this.guessCounter =  SPRINT.getNbGuesses(this.gameLevel);
        const updateSprint: UpdateSprint = this.createUpdateSprint(this.guessCounter, word,  this.timeLimit);
        io.in(this.matchId).emit("update_sprint", JSON.stringify(updateSprint));

        this.virtualDrawing.draw(io, game.drawing, this.gameLevel);
        
        this.timer = Date.now();
        this.timeout = setTimeout(() => {
            this.endMatch(io);
        }, this.timeLimit * 1000);
    }

    public async endTurn(io: SocketIO.Server): Promise<void> {
        this.reset(io);

        if (this.currentWord != "") { // currentWord is undefined at the first endTurn
            this.notifyWord(io);
        }

        this.currentWord = "";

        if (this.drawer.isVirtual) {
            const word: string = await gameDB.getRandomWord();
            this.startTurn(io, word);
        }
    }

    public guessRight(io: SocketIO.Server, username: string): void {
        const message: Message = Admin.createAdminMessage(username + " guessed the word.", this.matchId);
        io.in(this.matchId).emit("new_message", JSON.stringify(message));

        const score: number = this.calculateScore();
        this.updateScore(username, score);

        io.in(this.matchId).emit("update_players", JSON.stringify(this.players));

        this.endTurn(io);
    }
}