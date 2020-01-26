import { JsonController, Get, Param, Post } from "routing-controllers";

@JsonController("/user")
export class UserController {
    
    @Get("/:userId")
    test(@Param("userId") userId: string) {
        console.log("Bonjour " + userId);
        // Retrieve user from database
        return userId;
    }

    @Post("/:userId")
    createUser(@Param("userId") userId: string) {
        // Create new user in database
        return "User created!";
    }

}

// Ref : https://www.npmjs.com/package/routing-controllers