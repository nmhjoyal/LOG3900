const leoProfanity = require('leo-profanity');
const frenchBadwordsList = require('french-badwords-list');
leoProfanity.add(frenchBadwordsList.array);

export default class ChatFilter {

    public static filter(message: string): string {
        return leoProfanity.clean(message);
    }
}

// Ref: https://www.npmjs.com/package/leo-profanity