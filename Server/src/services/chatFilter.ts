const Filt = require('bad-words');
const frenchlist = require('french-badwords-list');
const filt = new Filt();
filt.addWords(...frenchlist);
export default class ChatFilter {

    static filter(message: string): string {
        return filt.clean(message);
    }
}