import { JsonController, Get, Param, Post, Body } from "routing-controllers";
import { User } from "../../models/User";


@JsonController("/user")
export class UserController {
    
    @Get("/:userId")
    test(@Param("userId") userId: string) {
        console.log("You sent : " + userId);
        // Retrieve user from database
        return "Get request done successfully";
    }

    @Post("/")
    createUser(@Body() body: User) {
        console.log(body.username);
        console.log(body.password);
        // const obj = JSON.parse(body);
        // Create new user in database
        // console.log("You sent : " + obj);
        return "Post request done successfully";
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers