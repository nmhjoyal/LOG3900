import Player from "../../models/player";
import { VPS, messages, CustomMessage, INSERT_HINT, ERROR } from "../../models/vp"

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

    public getStartMatchMessage(username: string): string {
        const cMsg: CustomMessage | undefined = messages.get(username);
        return (cMsg) ? cMsg.startMatch : ERROR.startMatch;
    }

    public getEndTurnMessage(username: string): string {
        const cMsg: CustomMessage | undefined = messages.get(username);
        return (cMsg) ? cMsg.endTurn : ERROR.endTurn;
    }

    public getHintMessage(username: string, hint: string): string {
        const cMsg: CustomMessage | undefined = messages.get(username);
        return (cMsg) ? cMsg.hint.replace(new RegExp(INSERT_HINT), hint) : ERROR.hint;
    }

    private getVPByUsername(username: string): Player | undefined{
        return VPS.find((vp) => vp.user.username === username);
    }

    private getRandomInt(max: number): number {
        return Math.floor(Math.random() * (Math.floor(max)));
    }
}