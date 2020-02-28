import { JsonController, Get, Param, Post, Body, Delete, Put } from "routing-controllers";
import { profileDB } from "../../services/Database/profileDB";
import PrivateProfile from "../../models/privateProfile";
import Feedback from "../../models/feedback";
import Admin from "../../models/admin";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/profile")
export class ProfileController {
   
    @Post("/create")
    public async createUser(@Body() profile: PrivateProfile) {

        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + profile.username + " created!"
        };

        if ((profile.username).toUpperCase() === (Admin.admin).toUpperCase()) {
            feedback.status = false;
            feedback.log_message = "Username Admin is reserved.";
        } else {
            try {
                await profileDB.createProfile(profile);
            } catch {
                feedback.status = false;
                feedback.log_message = "Could not create profile.";
            }
        }
        
        return feedback;
    }

    @Delete("/:userName")
    public async deleteUserInfos(@Param("userName") userName: string) {

        await profileDB.deleteProfile(userName);
        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + userName + " deleted!"
        };
        
        return feedback;
    }

    @Put("/update")
    public async updateProfile(@Body() profile: PrivateProfile) {

        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + profile.username + " updated!"
        };

        try {
            await profileDB.updateProfile(profile);
        } catch {
            feedback.status = false;
            feedback.log_message = "Could not update profile.";
        }
        
        return feedback;
    }

    @Get("/private/:userName")
    public async getPrivateUserInfos(@Param("userName") userName: string) {
        const profileRetrieved: PrivateProfile | null = await profileDB.getPrivateProfile(userName);
        return profileRetrieved;
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers