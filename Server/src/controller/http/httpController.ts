import { JsonController, Get, Param, Post, Body } from "routing-controllers";
import Profile from "../../models/profile";

/**
 * HTTPController is used only to manage user database and game database. 
 */
@JsonController("/profile")
export class HttpController {

    // Eventually 

    @Post("/createProfile")
    createUser(@Body() profile: Profile) {
        console.log(profile);
    }

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
}

// Ref : https://www.npmjs.com/package/routing-controllers