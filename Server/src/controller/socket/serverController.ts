import { OnConnect, SocketController, ConnectedSocket, OnDisconnect, MessageBody, OnMessage, SocketIO } from "socket-controllers";
import { serverHandler } from "../../services/serverHandler";
import SignIn from "../../models/signIn";
import PrivateProfile from "../../models/privateProfile";
 
@SocketController()
export class ServerController {
 
    @OnConnect()
    public connection(@ConnectedSocket() socket: SocketIO.Socket) {
        console.log("client connected");
    }
 
    @OnDisconnect()
    public disconnect(@ConnectedSocket() socket: SocketIO.Socket) {
        serverHandler.signOut(socket);
        console.log("client disconnected");
    }

    @OnMessage("sign_in")
    public async sign_in(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() signIn: SignIn) {
        socket.emit("user_signed_in", JSON.stringify(await serverHandler.signIn(socket, signIn)));
    }

    @OnMessage("sign_out")
    public async sign_out(@ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("user_signed_out", JSON.stringify(await serverHandler.signOut(socket)));
    }

    @OnMessage("update_profile")
    public async update_profile(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() profile: PrivateProfile) {
        socket.emit("profile_updated", JSON.stringify(await serverHandler.updateProfile(io, socket, profile)));
    }

    // @OnMessage("getroom_test")
    // public async getroom_test(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
    //     socket.emit("getroom_test_res", JSON.stringify(await roomDB.getRoom(roomId)));
    // }
}
// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme