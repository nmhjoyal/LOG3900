import { JsonController, Get, Param, Post, Body, HttpError } from "routing-controllers";
import Profile from "../../models/profile";
import { profileDB } from "../../services/Database/profileDB";

/**
 * HTTPController is used only to manage user database and game database. 
 */
@JsonController("/profile")
export class HttpController {

    @Get("/:userName")
    test(@Param("userName") userName: string) {
        let profileRetrieved : Profile = {
            firstname : "string",
            lastname : "string",
            username : "string",
            password : "string"
        }
        return profileRetrieved;
    }
   
    @Post("/create")
    public async createUser(@Body() profile: Profile) {
        const profileCreated: boolean = await profileDB.createProfile(profile);
        if (profileCreated) {
            return "Profile " + profile.username + " created!";
        } else {
            throw new HttpError(400);
        }
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers