import { JsonController, Post, Body } from "routing-controllers";
import { Feedback } from "../../models/feedback";
import { Game, Line } from "../../models/drawPoint";
import { gameDB } from "../../services/Database/gameDB";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/game")
export class GameController {
   
    @Post("/create")
    public async createGame(@Body() game: Game): Promise<Feedback> {
        // console.log(JSON.stringify(game));
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
            game.drawing.forEach((line: Line) => {
                line.DrawingAttributes = {
                    Color: line.DrawingAttributes.Color,
                    Width: line.DrawingAttributes.Width
                }
                for(let i = 0; i < line.StylusPoints.length; i++) {
                    line.StylusPoints[i] = {
                        X: line.StylusPoints[i].X,
                        Y: line.StylusPoints[i].Y
                    }
                }
            });
            try {
                await gameDB.createGame(game);
            } catch {
                feedback.status = false;
                feedback.log_message = "Could not create game.";
            }
        }
        console.log(feedback);
        return feedback;
    }
}