const LineByLine = require('n-readlines');

enum Difficulty {
    Easy = "Easy",
    Medium = "Medium",
    Hard = "Hard",
}

/**
 * The random words lists used by this class were created using 
 * REF : https://randomwordgenerator.com/pictionary.php
 */
export default class RandomWordGenerator {    
    public static generateChoices(): string[] {
        let words: string[] = [];

        words.push(RandomWordGenerator.getRandomWord(Difficulty.Easy));
        words.push(RandomWordGenerator.getRandomWord(Difficulty.Medium));
        words.push(RandomWordGenerator.getRandomWord(Difficulty.Hard));

        return words;
    }

    private static getRandomWord(difficulty: Difficulty): string {
        const liner = new LineByLine('../../services/wordGenerator/wordList' + difficulty + '.txt');
        const numberOfLines = liner.next();
        const randomLine = Math.floor(Math.random() * numberOfLines);

        for (let i = 0; i < randomLine; i++) {
            liner.next();
        }
        return liner.next();
    }
}

// REF : https://www.npmjs.com/package/n-readlines