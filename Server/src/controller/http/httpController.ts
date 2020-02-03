import { JsonController, Get, Param, Post, Body } from "routing-controllers";
import User from "../../models/user";

/**
 * HTTPController is used only to manage user database and game database. 
 */
@JsonController("/user")
export class HttpController {

    // Eventually 

    @Post("/createUser")
    createUser(@Body() user: User) {

        // console.log(body)
        // console.log(body.username);
        // console.log(body.password);
        // // this.room.addUser(body);
        // // Create new user in database
        // return body.username + " added";
    }

    @Get("/:userName")
    test(@Param("userName") userName: string) {
        console.log("You sent : " + userName);
        /*
        let userRetrieved: User | undefined = this.room.getUser(userName);
        if (userRetrieved) {
            return userRetrieved;
        } else {
            return "User does not exist";
        }
        */
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers