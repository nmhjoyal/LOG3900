export default class RandomIdGenerator {
    public static generate(): string {
        return '_' + Math.random().toString(36).substr(2, 9);
    }
}