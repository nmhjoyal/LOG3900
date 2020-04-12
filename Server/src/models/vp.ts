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
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: LORD_BANANA,
            avatar: "BANANA"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: SGT_STRAWBERRY,
            avatar: "STRAWBERRY"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: LADY_WATERMELON,
            avatar: "WATERMELON"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: MASTER_GRAPE,
            avatar: "GRAPE"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: GENTLEMAN_KIWI,
            avatar: "KIWI"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: MADAM_ORANGE,
            avatar: "ORANGE"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: SIR_PINEAPPLE,
            avatar: "PINEAPPLE"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: LITTLE_APPLE,
            avatar: "APPLE"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: MISS_LEMON,
            avatar: "LEMON"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: DRE_CHERRY,
            avatar: "CHERRY"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    },
    {
        user: {
            username: PIRATE_PEAR,
            avatar: "PEAR"
        },
        score: {
            scoreTotal: 0,
            scoreTurn: 0
        },
        isVirtual: true
    }
];


export interface CustomMessage {
    startMatch: string,
    endTurn: string[],
    hint: string
}

// Used to insert hint (str.replace("_")) in the function creating the message content.
export const INSERT_HINT: string = "_";

// Used for unexpected error of not finding messages of a username in the map, should not happen.
export const ERROR: CustomMessage = {
    startMatch: "The game is started.",
    endTurn: ["The turn is done."],
    hint: "The hint is " + INSERT_HINT + "."
}

export const messages: Map<string /*username*/, CustomMessage> = new Map<string, CustomMessage>([
    [
        MR_AVOCADO,
        {
            startMatch: "Hello Everyone, I hope you are all doing great and ready to make friends :) ",
            endTurn: [
                "Aww, Too bad! I'm sure you'll do better next time.",
                "Well done, don't lose hope. You are great!",
                "Don't worry, everything will be alright.",
                "It's okay to lose sometimes. You know what they say: one step back, two steps forward",
                "You might have lost the round, but you definitely won a friend.",
                "It's just a game. You are worth more than that",
                "Can we still be friends ?",
                "You make me smile.",
                "What do you say we go for coffee after this game ?",
                "I got your back"
            ],
            hint: "You know, it's only because I consider you a friend. There you go :" + INSERT_HINT
        },
    ],
    [
        LORD_BANANA,
        {
            startMatch: "First of all, you have to refer to me as Lord. Now let's play.",
            endTurn: [
                "See? You should listen to me",
                "The next time you don't follow the order, I will make sure you suffer from it.",
                "1, 2, 3, 4. Praise the Lord Banana! 5, 6, 7, 8. I will listen to what he says!",
                "I'm the one with all the answers!",
                "Lord Banana is the King of the World and you should act like it!",
                "You wish you could be me. I'm too charismatic.",
                "Next wrong answer and you're out!",
                "It is not truth that matters, but victory. - Lord Banana",
                "If you win, you need not have to explain...If you lose, you should not be there to explain!",
                "As in everything, Lord Banana is the best instructor."
            ],
            hint: "Obstacles do not exist to be surrendered to, but only to be broken. So here's a hint:" + INSERT_HINT
        },
    ],
    [
        SGT_STRAWBERRY,
        {
            startMatch: "TODO",
            endTurn: [
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO"
            ],
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        LADY_WATERMELON,
        {
            startMatch: "Let's get this started then, tea time is soon!",
            endTurn: [
                "The Queen of England's dogs could've done better love.",
                "I must admit, quite an entertaining round that was.",
                "Blimey! Jolly good show my darlings.",
                "Nicely done dear.",
                "It's about time we've finished. I'm simply knackered.",
                "Well done! Let's get this bloody game over with already...",
                "Right then, I need to see a man about a dog now. On with it!",
                "Blimey! Done already?",
                "This is simply bollocks, time just does not seem to be on my side.",
                "Brilliant darling, simply brilliant."
            ],
            hint: "There's no need to get cheeky, have a gander at this then: " + INSERT_HINT
        },
    ],
    [
        MASTER_GRAPE,
        {
            startMatch: "TODO",
            endTurn: [
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO"
            ],
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        GENTLEMAN_KIWI,
        {
            startMatch: "TODO",
            endTurn: [
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO"
            ],
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        MADAM_ORANGE,
        {
            startMatch: "TODO",
            endTurn: [
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO"
            ],
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        SIR_PINEAPPLE,
        {
            startMatch: "TODO",
            endTurn: [
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO",
                "TODO"
            ],
            hint: "TODO" + INSERT_HINT
        },
    ],
    [
        LITTLE_APPLE,
        {
            startMatch: "Yay! I'm so excited to play! Let's win!",
            endTurn: [
                "I don't understand, I thought I had more time :(",
                "Wow, you type soooo fast!",
                "My mom said I could play for only 5 more minutes. Let's hurry!",
                "During nap time I had a dream I would win this game.",
                "My daddy said that if you close your eyes and count to 10 you won't feel angry anymore!",
                "My friend Eddie told me dogs with floppy ears can fly if they shake their heads SUPER SUPER FAST! Like a helicopter.",
                "Are we done yet? I'm boredddd",
                "My mommy said she loves me thiiiiiis much. Do you have a mommy?",
                "My cat Skittle scratched my big brother on the arm and there was BLOOD",
                "That was sooo fun, let's play again!"
            ],
            hint: "I asked my mommy, she said it's something like this: " + INSERT_HINT
        },
    ],
    [
        MISS_LEMON,
        {
            startMatch: "Buongiorno, I wish you guys a good match, the winner will get my beautiful lasagna.",
            endTurn: [
                "I wish you would all come to Italia, life is good here in Sicilia.",
                "I would love to guide you trough Venezia, it is a beautiful city to discover.",
                "I don't understand the point of this game, at least the winner is going to get my lasagna.",
                "Guys tell me about Montreal, I would love to come visit you one day.",
                "My son lives in Montreal he told me the Little Italy district is great.",
                "My country has a great history, you must have heard of the Roman Empire.",
                "I have to leave soon the mercato is closing in an hour and I need bread.",
                "I heard the weather is not great in Montreal, here in Italia it is always sunny.",
                "I hope my lasagna doesn't burn while I play this game with you guys.",
                "I love cooking for my friends, I would love for you to taste my dishes"
            ],
            hint: "The hint is " + INSERT_HINT + ". Now, just let me make sure my lasagna is not burning."
        },
    ],
    [
        DRE_CHERRY,
        {
            startMatch: "Quickly, let's start! I have a patient coming in 15 minutes.",
            endTurn: [
                "I'm getting sick... of losing against a bunch of parasites like you.",
                "Did you know that winning makes you feel better?",
                "Did everyone get their 8 hours of sleep? Because you seem a little slow..",
                "I COMPLETELY FORGOT ABOUT THE PATIENT WAITING IN THE ROOM NEXT TO ME!",
                "So how's allergy season treating everyone?",
                "Did you know that searching for your symptoms on Google won't reassure you?",
                "I hope everyone is washing their hands!",
                "I've been so busy with this app lately. I've been playing for 3 hours now and my shift started 4 hours ago...",
                "Can we stop and discuss the ongoing pandemic?",
                "I should really be working on my 'saving lives' skills.."
            ],
            hint: "No, you won't die if you lose. Take this hint, it will make you feel better: " + INSERT_HINT
        },
    ],
    [
        PIRATE_PEAR,
        {
            startMatch: "Yo ho ho matey, we're ready to sail.",
            endTurn: [
                "The sea is dangerous but I hope I never get to visit Davy Jones's Locker.",
                "If I don't win this game, the one who dared disrespecting me will walk the plank.",
                "When I win, i'm not sharing the treasure.",
                "Arr, next round i'll crush ye bernacles!",
                "Blimey, I'd rather be marooned than losing to a bunch of landlubbers like you!",
                "I swear you'll feed the fish if you keep this up. ",
                "Hearties, we will celebrate my victory with a clap of thunder!",
                "I'm an old salt, you won't believe the things i saw at sea.",
                "Corsairs couldn't defeat me. What makes you believe you can? ",
                "Arrrrrr! You can't horneswaggle me!"
            ],
            hint: "Aye, navigating through troubled water? Let me help with this hint: " + INSERT_HINT
        },
    ],
]);