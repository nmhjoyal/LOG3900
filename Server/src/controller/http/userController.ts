import { JsonController, Get, Param, Post, Body } from "routing-controllers";
import { User } from "../../models/user";
// import { Room } from "../../services/room";


@JsonController("/user")
export class UserController {

    // private room: Room;

    public constructor() {
        // this.room = new Room("Room1");
    }

    @Post("/add")
    createUser(@Body() body: User) {
        console.log(body)
        console.log(body.username);
        console.log(body.password);
        // this.room.addUser(body);
        // Create new user in database
        return body.username + " added";
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