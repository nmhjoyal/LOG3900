import Match from "./matchAbstract";
import PublicProfile from "../../models/publicProfile";
import ChatHandler from "../chatHandler";
import { CreateMatch,} from "../../models/match";
import Player from "../../models/player";
import { gameDB } from "../Database/gameDB";
import { Game } from "../../models/drawPoint";
import { Message } from "../../models/message";
import Admin from "../../models/admin";
import { Feedback } from "../../models/feedback";
import { freeForAllSettings } from "../../models/matchMode";

export default class FreeForAll extends Match {

    public constructor(matchId: string, user: PublicProfile, createMatch: CreateMatch, chatHandler: ChatHandler) {
        super(matchId, user, createMatch, chatHandler, freeForAllSettings);
    }

    public async startTurn(io: SocketIO.Server, word: string, isVirtual: boolean): Promise<void> {
        this.currentWord = word;
        io.in(this.matchId).emit("turn_started", this.createStartTurn(this.currentWord));
        
        if (isVirtual) {
            const game: Game = await gameDB.getGame(word);
            this.virtualDrawing.draw(io, game.drawing, game.level);
        }
        
        this.timer = Date.now();
        this.timeout = setTimeout(() => {
            if(isVirtual) {
                this.virtualDrawing.clear(io);
            }
            this.endTurn(io, false);
        }, this.timeLimit * 1000);
    }

    public async endTurn(io: SocketIO.Server, drawerLeft: boolean): Promise<void> {
        clearTimeout(this.timeout);
        let matchIsEnded: boolean = false;

        if (!drawerLeft){
            let oldDrawer: Player | undefined = this.getPlayer(this.drawer);
            if (oldDrawer) {
                this.assignDrawer(oldDrawer);

                if (this.round == this.nbRounds + 1) {
                    matchIsEnded = true;
                }
            }
        }

        if (matchIsEnded) {
            this.endMatch(io);
        } else {
            this.endTurnGeneral(io);
        }
    }

    public guess(io: SocketIO.Server, guess: string, username: string): Feedback {
        let feedback: Feedback = { status: false, log_message: "" };

        if (this.currentWord != "") {
            if (this.drawer != username) {
                if(guess == this.currentWord) {
                    const message: Message = Admin.createAdminMessage(username + " guessed the word.", this.matchId);
                    io.in(this.matchId).emit("new_message", JSON.stringify(message));
        
                    const score: number = Math.round((Date.now() - this.timer) / 1000) * 10;
                    this.updateScore(username, score);
                    this.updateScore(this.drawer, Math.round(score / this.players.length));

                    io.in(this.matchId).emit("update_players", JSON.stringify(this.players));
        
                    if(this.everyoneHasGuessed()) {
                        this.endTurn(io, false);
                    }
                } else {
                    feedback.log_message = "Your guess is wrong.";
                }
            } else {
                feedback.log_message = "The player drawing is not supposed to guess.";
            }
        } else  {
            feedback.log_message = "The word guessed is empty.";
        }

        return feedback;
    }
}