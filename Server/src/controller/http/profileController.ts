import { JsonController, Get, Param, Post, Body, HttpError, Delete, NotFoundError } from "routing-controllers";
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
        try {
            await profileDB.createProfile(profile);
        } catch {
            throw new HttpError(400);
        }
        // Querry worked
        return "Profile " + profile.username + " created!";
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
    public async deleteUserInfos(@Param("userName") userName: string) {
        try {
            await profileDB.deleteProfile(userName);
        } catch {
            throw new HttpError(400);
        }

        const privateProfile: PrivateProfile = {
            firstname : "string",
            lastname : "string",
            username : "string",
            password : "string",
            avatar : "string"/*String for the moment eventually needs to be image*/
        }

        const publicrter:PublicProfile = privateProfile;
        console.log(publicrter)
        // Querry worked
        return "Profile " + userName + " deleted!"
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers