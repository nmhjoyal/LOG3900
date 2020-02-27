import { JsonController, Get, Param, Post, Body, HttpError, Delete, NotFoundError, Put } from "routing-controllers";
import { profileDB } from "../../services/Database/profileDB";
import PublicProfile from "../../models/publicProfile";
import PrivateProfile from "../../models/privateProfile";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/profile")
export class ProfileController {
   
    @Post("/create")
    public async createUser(@Body() profile: PrivateProfile) {
        try {
            await profileDB.createProfile(profile);
        } catch {
            throw new HttpError(400);
        }
        // Querry worked
        return "Profile " + profile.username + " created!";
    }

    @Delete("/:userName")
    public async deleteUserInfos(@Param("userName") userName: string) {
        await profileDB.deleteProfile(userName);
        return "Profile " + userName + " deleted!"
    }

    @Put("/update")
    public async updateProfile(@Body() profile: PrivateProfile) {
        try {
            await profileDB.updateProfile(profile);
        } catch {
            throw new HttpError(400);
        }
        // Querry worked
        return "Profile " + profile.username + " updated!";
    }

    @Get("/private/:userName")
    public async getPrivateUserInfos(@Param("userName") userName: string) {
        const profileRetrieved: PrivateProfile | null = await profileDB.getPrivateProfile(userName);
        return profileRetrieved;
    }

    // TEST DB : 
    // @Get("/public/:userName")
    // public async getPublicUserInfos(@Param("userName") userName: string) {
    //     const profileRetrieved: PublicProfile | null = await profileDB.getPublicProfile(userName);
    //     return profileRetrieved;
    // }
}

// Ref : https://www.npmjs.com/package/routing-controllers