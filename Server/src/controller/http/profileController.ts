import { JsonController, Get, Param, Post, Body, Delete } from "routing-controllers";
import { profileDB } from "../../services/Database/profileDB";
import PrivateProfile from "../../models/privateProfile";
import { Feedback } from "../../models/feedback";
import Admin from "../../models/admin";
import { roomDB } from "../../services/Database/roomDB";
import PublicProfile from "../../models/publicProfile";
import { serverHandler } from "../../services/serverHandler";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/profile")
export class ProfileController {
   
    @Post("/create")
    public async createUser(@Body() profile: PrivateProfile): Promise<Feedback> {

        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + profile.username + " created!"
        };

        if ((profile.username).toUpperCase() === (Admin.admin).toUpperCase()) {
            feedback.status = false;
            feedback.log_message = "Username Admin is reserved.";
        } else {
            try {
                const generalRoomId: string = "General";
                profile.rooms_joined.push(generalRoomId);
                const publicProfile: PublicProfile = {
                    username: profile.username,
                    avatar: profile.avatar
                };
                await profileDB.createProfile(profile);
                await roomDB.mapAvatar(publicProfile, generalRoomId);
            } catch {
                feedback.status = false;
                feedback.log_message = "Could not create profile.";
            }
        }
        
        return feedback;
    }

    @Delete("/:userName")
    public async deleteUserInfos(@Param("userName") username: string): Promise<Feedback> {

        await profileDB.deleteProfile(username);
        serverHandler.users.delete(username);
        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + username + " deleted!"
        };
        
        return feedback;
    }

    @Get("/private/:userName")
    public async getPrivateUserInfos(@Param("userName") userName: string): Promise<PrivateProfile | null> {
        const profileRetrieved: PrivateProfile | null = await profileDB.getPrivateProfile(userName);
        return profileRetrieved;
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers