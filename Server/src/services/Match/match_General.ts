import { Room } from "../../models/room";
import { Feedback } from "../../models/feedback";
import PublicProfile from "../../models/publicProfile";

export default abstract class Match {
    protected players: Map<string, number>; /* username, score */
    protected nbRounds: number;
    protected letterReveal: boolean;
    // Chat associated to the game.
    protected matchRoom: Room;
    protected isStarted: boolean;

    protected constructor(socket: SocketIO.Socket, randomId: string, nbRounds: number) {
        this.matchRoom.id = randomId;
        // this.joinRoom
        this.nbRounds = nbRounds;
        this.isStarted = false;
    }

    public joinMatch(socket: SocketIO.Socket, user: PublicProfile): Feedback {
        let feedback: Feedback = {
            status: true,
            log_message: ""
        };

        if (!this.isStarted) {
            // this.joinRoom
            feedback.status = true;
            feedback.log_message = "You joined the match.";
        } else {
            feedback.status = false;
            feedback.log_message = "The match is already started.";
        }

        return feedback;
    }

    public startMatch(): void {
        this.isStarted = true;
    }

    public endMatch(): void {

    }

    public startRound(): void {

    }

    public endRound(): void {

    }

    public draw(): void { 

    }
    
    // private joinRoom(socket: SocketIO.Socket) {
    //     socket.join(this.matchRoom.id);
    //     this.matchRoom.avatars.set();
    //     this.players = [socket.id];
    // }

}