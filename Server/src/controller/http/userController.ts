import { JsonController, Get, Param, Post, Body } from "routing-controllers";
import { User } from "../../models/user";


@JsonController("/user")
export class UserController {

    @Post("/")
    createUser(@Body() body: User) {
        console.log(body.username);
        console.log(body.password);
        // const obj = JSON.parse(body);
        // Create new user in database
        // console.log("You sent : " + obj);
        return body.username + " added";
    }

    @Get("/:userId")
    test(@Param("userId") userId: string) {
        console.log("You sent : " + userId);
        // Retrieve user from database
        return "Get request done successfully";
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers