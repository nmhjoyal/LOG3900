export default class RandomMatchIdGenerator {
    public static prefix: string = "_match_";
    public static generate(): string {
        return this.prefix + Math.random().toString(36).substr(2, 9);
    }
}