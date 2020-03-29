import PublicProfile from "./publicProfile";

export default interface Player {
    user: PublicProfile
    isHost: boolean
    isVirtual: boolean
}