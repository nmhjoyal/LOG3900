const LineByLine = require('n-readlines');

/**
 * The random words lists used by this class were created using 
 * REF : https://randomwordgenerator.com/pictionary.php
 */
export default class RandomWordGenerator {
    public static generate(qty: number): string[] {
        let words: string[] = [];

        for (let i = 0; i < qty; i++) {
            words.push(RandomWordGenerator.getRandomWord())
        }

        return words;
    }

    private static getRandomWord(): string {
        const liner = new LineByLine('../../services/wordGenerator/wordListHard.txt');
        const numberOfLines = liner.next();
        const randomLine = Math.floor(Math.random() * numberOfLines);

        for (let i = 0; i < randomLine; i++) {
            liner.next();
        }
        return liner.next();
    }
}

// REF : https://www.npmjs.com/package/n-readlines