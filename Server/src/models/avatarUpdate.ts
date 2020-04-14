import PublicProfile from "./publicProfile";

export default interface AvatarUpdate {
    roomId : string 
    updatedProfile : PublicProfile // contains the new avatar
} 