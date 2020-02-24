import { JsonController, Get, Param, Post, Body, HttpError, Delete } from "routing-controllers";
import Profile from "../../models/privateProfile";
import { profileDB } from "../../services/Database/profileDB";
import PublicProfile from "../../models/publicProfile";
import PrivateProfile from "../../models/privateProfile";

/**
 * HTTPController is used only to manage user database and game database. 
 */
@JsonController("/profile")
export class ProfileController {
   
    @Post("/create")
    public async createUser(@Body() profile: Profile) {
        const profileCreated: boolean = await profileDB.createProfile(profile);
        if (profileCreated) {
            return "Profile " + profile.username + " created!";
        } else {
            throw new HttpError(400);
        }
    }

    @Get("/public/:userName")
    public getPublicUserInfos(@Param("userName") userName: string) {
        let profileRetrieved : PublicProfile = {
            username : "string",
            avatar : "string"
        }
        return profileRetrieved;
    }

    @Get("/private/:userName")
    public getPrivateUserInfos(@Param("userName") userName: string) {
        let profileRetrieved : PrivateProfile = {
            firstname : "string",
            lastname : "string",
            username : "string",
            password : "string",
            avatar : "string"
        }
        return profileRetrieved;
    }

    @Delete("/:userName")
    public deleteUserInfos(@Param("userName") userName: string) {

    }
}

// Ref : https://www.npmjs.com/package/routing-controllers