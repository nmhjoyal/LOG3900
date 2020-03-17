import { JsonController, Post, Body } from "routing-controllers";
import { Feedback } from "../../models/feedback";
import { Game, Stroke, StylusPoint } from "../../models/drawPoint";
import { gameDB } from "../../services/Database/gameDB";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/game")
export class GameController {
   
    @Post("/create")
    public async createGame(@Body() game: Game): Promise<Feedback> {
        let feedback: Feedback = {
            status: true,
            log_message: "Game created!"
        };
        for(let i = 0; i < game.clues.length; i++) {
            if(game.clues[i] == "") {
                game.clues.splice(i, 1);
            }
        }
        if(game.drawing.length == 0) {
            feedback.status = false;
            feedback.log_message =  "You must provide a drawing";
        } else if(game.word == "") {
            feedback.status = false;
            feedback.log_message = "You must provide a word to guess";
        } else if(game.clues.length == 0) {
            feedback.status = false;
            feedback.log_message = "You must provide a least one clue";
        } else {
            let top: number = 0;
            game.drawing.forEach((stroke: Stroke) => {
                stroke.DrawingAttributes = {
                    Color: stroke.DrawingAttributes.Color,
                    Width: stroke.DrawingAttributes.Width,
                    Height: stroke.DrawingAttributes.Height,
                    Top: top++
                };
                stroke.StylusPoints.forEach((stylusPoint: StylusPoint, i: number) => {
                    stroke.StylusPoints[i] = {
                        X: stylusPoint.X,
                        Y: stylusPoint.Y
                    }
                });
            });
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