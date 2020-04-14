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
import { statsDB } from "../../services/Database/statsDB";
import { StatsClient } from "../../models/stats";

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
        } else if(profile.username.length < 4 || profile.username.length > 10) {
            feedback.status = false;
            feedback.log_message = "Username has to be between 4 and 10 characters";
        }else if(profile.password.length < 4 || profile.password.length > 10) {
            feedback.status = false;
            feedback.log_message = "Password has to be between 4 and 10 characters";
        } else {
            try {
                const generalRoomId: string = "General";
                profile.rooms_joined = [generalRoomId];
                const publicProfile: PublicProfile = {
                    username: profile.username,
                    avatar: profile.avatar
                };
                await profileDB.createProfile(profile);
                rankingDB.createRank(profile.username);
                statsDB.createStats(profile.username);
                await roomDB.mapAvatar(publicProfile, generalRoomId);
            } catch {
                feedback.status = false;
                feedback.log_message = "Could not create profile.";
            }
        }
        
        return feedback;
    }

    @Delete("/:username")
    public deleteUserInfos(@Param("username") username: string): Feedback {

        profileDB.deleteProfile(username);
        rankingDB.deleteRank(username);
        statsDB.deleteStats(username);
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
    public async getRanks(@Param("username") username: string, @Param("matchMode") matchMode: MatchMode): Promise<Rank[]> {
        return await rankingDB.getRanks(username, matchMode);
    }

    @Get("/stats/:username")
    public async getStats(@Param("username") username: string): Promise<StatsClient> {
        return await statsDB.getStats(username);
    }
}

// Ref : https://www.npmjs.com/package/routing-controllers