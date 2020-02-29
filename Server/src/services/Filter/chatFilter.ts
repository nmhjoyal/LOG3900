import Filt from "bad-words";
import FrenchList from "french-badwords-list";

class ChatFilter {

    private filt: any;

    public constructor () {
        this.filt = new Filt();
        this.filt.addWords(...FrenchList);
    }

    private filter(message: string): string {
        return this.filt.clean(message);
    }
}

export var chatFilter: ChatFilter = new ChatFilter();