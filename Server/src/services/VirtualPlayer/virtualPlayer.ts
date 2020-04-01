import Player from "../../models/player";
import { VPS, messages, CustomMessage, INSERT_HINT, ERROR } from "../../models/vp"
import { Message } from "../../models/message";

export default class VirtualPlayer {
    private vps: Player[];

    public constructor() {
        this.vps = VPS;
    }

    public create(): Player {
        return this.vps.slice(this.getRandomInt(this.vps.length), 1)[0];
    }

    public newAvailableVP(username: string): void {
        const vpToAddBack: Player | undefined = this.getVPByUsername(username);
        if (vpToAddBack) {
            this.vps.push(vpToAddBack);
        }
    }

    public getStartMatchMessage(username: string, roomId: string): Message {
        const cMsg: CustomMessage | undefined = messages.get(username);
        const content: string = (cMsg) ? cMsg.startMatch : ERROR.startMatch;
       
        return this.createMessageObj(username, content, roomId);
    }

    public getEndTurnMessage(username: string, roomId: string): Message {
        const cMsg: CustomMessage | undefined = messages.get(username);
        const content: string = (cMsg) ? cMsg.endTurn : ERROR.endTurn;

        return this.createMessageObj(username, content, roomId);
    }

    public getHintMessage(username: string, hint: string, roomId: string): Message {
        const cMsg: CustomMessage | undefined = messages.get(username);
        const content: string = (cMsg) ? cMsg.hint.replace(new RegExp(INSERT_HINT), hint) : 
                                         ERROR.hint;
        
        return this.createMessageObj(username, content, roomId);
    }
    
    private createMessageObj(username: string, content: string, roomId: string): Message {
        return {
            username: username,
            content: content,
            date: Date.now(),
            roomId: roomId
        }
    }

    private getVPByUsername(username: string): Player | undefined{
        return VPS.find((vp) => vp.user.username === username);
    }

    private getRandomInt(max: number): number {
        return Math.floor(Math.random() * (Math.floor(max)));
    }
}