import { JsonController, Post, Body } from "routing-controllers";
import { Feedback } from "../../models/feedback";
import { CreateGame, Stroke, StylusPoint, Game } from "../../models/drawPoint";
import { gameDB } from "../../services/Database/gameDB";
import { Utils } from "../../services/Drawing/utils";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/game")
export class GameController {
   
    @Post("/create")
    public async createGame(@Body() createGame: CreateGame): Promise<Feedback> {
        let feedback: Feedback = {
            status: true,
            log_message: "Game created!"
        };
        for(let i = 0; i < createGame.clues.length; i++) {
            if(createGame.clues[i] == "") {
                createGame.clues.splice(i, 1);
            }
        }
        if(createGame.drawing.length == 0) {
            feedback.status = false;
            feedback.log_message =  "You must provide a drawing";
        } else if(createGame.word == "") {
            feedback.status = false;
            feedback.log_message = "You must provide a word to guess";
        } else if(createGame.clues.length == 0) {
            feedback.status = false;
            feedback.log_message = "You must provide a least one clue";
        } else {
            let top: number = 0;
            createGame.drawing.forEach((stroke: Stroke) => {
                stroke.DrawingAttributes = {
                    Color: stroke.DrawingAttributes.Color,
                    Width: stroke.DrawingAttributes.Width,
                    StylusTip: stroke.DrawingAttributes.StylusTip,
                    Top: top++
                };
                stroke.StylusPoints.forEach((stylusPoint: StylusPoint, i: number) => {
                    stroke.StylusPoints[i] = {
                        X: stylusPoint.X,
                        Y: stylusPoint.Y
                    }
                });
            });
            Utils.sort(createGame.drawing, createGame.mode, createGame.option);
            const game: Game = {
                word: createGame.word,
                drawing: createGame.drawing,
                clues: createGame.clues,
                level: createGame.level
            };
            try {
                await gameDB.createGame(game);
            } catch {
                feedback.status = false;
                feedback.log_message = "This drawing already exists";
            }
        }
        return feedback;
    }
}