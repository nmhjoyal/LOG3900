import { JsonController, Param, Post, Body, HttpError, Delete } from "routing-controllers";
import { profileDB } from "../../services/Database/profileDB";
import PrivateProfile from "../../models/privateProfile";

/**
 * HTTPController is used only to manage user database and game database. 
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

    // @Put("/update/firstname/:username/:new")

    // TEST DB : 
    // @Get("/public/:userName")
    // public async getPublicUserInfos(@Param("userName") userName: string) {
    //     const profileRetrieved: PublicProfile | null = await profileDB.getPublicProfile(userName);
    //     return profileRetrieved;
    // }

    // @Get("/private/:userName")
    // public async getPrivateUserInfos(@Param("userName") userName: string) {
    //     const profileRetrieved: PrivateProfile | null = await profileDB.getPrivateProfile(userName);
    //     return profileRetrieved;
    // }
}

// Ref : https://www.npmjs.com/package/routing-controllers