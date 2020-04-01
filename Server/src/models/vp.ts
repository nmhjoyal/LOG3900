import Player from "./player";

// Usernames
// REF: https://en.wikipedia.org/wiki/English_honorifics
const MR_AVOCADO: string = "Mr Avocado";            // content gentil
const LORD_BANANA: string = "Lord Banana";          // dictateur, donne des ordres
const SGT_STRAWBERRY: string = "Sgt Strawberry";    // autoritaire
const LADY_WATERMELON: string = "Lady Watermelon";  // british 
const MASTER_GRAPE: string = "Master Grape";        // professeur, donne des conseils
const GENTLEMAN_KIWI: string = "Gentleman Kiwi";    // poli
const MADAM_ORANGE: string = "Madam Orange";        // furieuse
const SIR_PINEAPPLE: string = "Sir Pineapple";      // prétentieux
const LITTLE_APPLE: string = "Little Apple";        // enfant, joueur et naif
const MISS_LEMON: string = "Miss Lemon";            // italienne 
const DRE_CHERRY: string = "Dre Cherry";            // analogies avec la médecine
const PIRATE_PEAR: string = "Pirate Pear";          // https://www.piratevoyages.com/pirate-lingo/

export const VPS: Player[] = [
    {
        user: {
            username: MR_AVOCADO,
            avatar: "AVOCADO"
        },
        isVirtual: true
    },
    {
        user: {
            username: LORD_BANANA,
            avatar: "BANANA"
        },
        isVirtual: true
    },
    {
        user: {
            username: SGT_STRAWBERRY,
            avatar: "STRAWBERRY"
        },
        isVirtual: true
    },
    {
        user: {
            username: LADY_WATERMELON,
            avatar: "WATERMELON"
        },
        isVirtual: true
    },
    {
        user: {
            username: MASTER_GRAPE,
            avatar: "GRAPE"
        },
        isVirtual: true
    },
    {
        user: {
            username: GENTLEMAN_KIWI,
            avatar: "KIWI"
        },
        isVirtual: true
    },
    {
        user: {
            username: MADAM_ORANGE,
            avatar: "ORANGE"
        },
        isVirtual: true
    },
    {
        user: {
            username: SIR_PINEAPPLE,
            avatar: "PINEAPPLE"
        },
        isVirtual: true
    },
    {
        user: {
            username: LITTLE_APPLE,
            avatar: "APPLE"
        },
        isVirtual: true
    },
    {
        user: {
            username: MISS_LEMON,
            avatar: "LEMON"
        },
        isVirtual: true
    },
    {
        user: {
            username: DRE_CHERRY,
            avatar: "CHERRY"
        },
        isVirtual: true
    },
    {
        user: {
            username: PIRATE_PEAR,
            avatar: "PEAR"
        },
        isVirtual: true
    }
];


export interface CustomMessage {
    startMatch: string,
    endTurn: string,
    hint: string
}

// Used to insert hint (str.replace("_")) in the function creating the message content.
export const INSERT_HINT: string = "_";

// Used for unexpected error of not finding messages of a username in the map, should not happen.
export const ERROR: CustomMessage = {
    startMatch: "The game is started.",
    endTurn: "The turn is done.",
    hint: "The hint is " + INSERT_HINT + "."
}

export const messages: Map<string /*username*/, CustomMessage> = new Map<string, CustomMessage>([
    [
        MR_AVOCADO,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        LORD_BANANA,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        SGT_STRAWBERRY,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        LADY_WATERMELON,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        MASTER_GRAPE,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        GENTLEMAN_KIWI,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        MADAM_ORANGE,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        SIR_PINEAPPLE,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        LITTLE_APPLE,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        MISS_LEMON,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        DRE_CHERRY,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        PIRATE_PEAR,
        {
            startMatch: "TODO",
            endTurn: "TODO",
            hint: "TODO" + INSERT_HINT
        },
    ],
]);