import { JsonController, Get, Param, Post, Body, Delete } from "routing-controllers";
import { profileDB } from "../../services/Database/profileDB";
import PrivateProfile from "../../models/privateProfile";
import { Feedback } from "../../models/feedback";
import Admin from "../../models/admin";
import { roomDB } from "../../services/Database/roomDB";
import PublicProfile from "../../models/publicProfile";
import { serverHandler } from "../../services/serverHandler";
import { Rank } from "../../models/rank";
import { rankingDB } from "../../services/Database/rankingDB";
import { MatchMode } from "../../models/matchMode";

/**
 * ProfileController is used to manage user profiles in the database. 
 */
@JsonController("/profile")
export class ProfileController {
   
    @Post("/create")
    public async createUser(@Body() profile: PrivateProfile): Promise<Feedback> {

        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + profile.username + " created!"
        };

        if ((profile.username).toUpperCase() === (Admin.admin).toUpperCase()) {
            feedback.status = false;
            feedback.log_message = "Username Admin is reserved.";
        } else {
            try {
                const generalRoomId: string = "General";
                profile.rooms_joined = [generalRoomId];
                const publicProfile: PublicProfile = {
                    username: profile.username,
                    avatar: profile.avatar
                };
                await profileDB.createProfile(profile);
                // await statsDB.createStats(profile.username);
                await roomDB.mapAvatar(publicProfile, generalRoomId);
            } catch {
                feedback.status = false;
                feedback.log_message = "Could not create profile.";
            }
        }
        
        return feedback;
    }

    @Delete("/:username")
    public async deleteUserInfos(@Param("username") username: string): Promise<Feedback> {

        await profileDB.deleteProfile(username);
        serverHandler.users.delete(username);
        let feedback: Feedback = {
            status: true,
            log_message: "Profile " + username + " deleted!"
        };
        
        return feedback;
    }

    @Get("/private/:username")
    public async getPrivateUserInfos(@Param("username") username: string): Promise<PrivateProfile | null> {
        const profileRetrieved: PrivateProfile | null = await profileDB.getPrivateProfile(username);
        return profileRetrieved;
    }

    @Get("/rank/:username/:matchMode")
    public async getRankings(@Param("username") username: string, @Param("matchMode") matchMode: MatchMode): Promise<Rank[]> {
        return await rankingDB.getRankings(username, matchMode);
    }

    // @Get("/stats/:username")
    // public async getStats(): Promise<Stats> {
        
    // }
}

// Ref : https://www.npmjs.com/package/routing-controllers