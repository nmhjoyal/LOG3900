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
    public disconnect(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        console.log("client disconnected");
    }

    @OnMessage("disconnecting")
    public disconnecting(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        console.log("client disconnecting");
        // console.log(socket.rooms);
        // var clonedRooms = Object.keys(socket.rooms).slice();
        // setTimeout(function() {
        //     console.log("emptyarray:", socket.rooms); // empty array
        //     console.log("rooms : ", clonedRooms);
        // }, 100); https://github.com/socketio/socket.io/pull/2332
        serverHandler.signOut(io, socket);
    }

    @OnMessage("sign_in")
    public async sign_in(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() signIn: SignIn) {
        socket.emit("user_signed_in", JSON.stringify(await serverHandler.signIn(socket, signIn)));
    }

    @OnMessage("sign_out")
    public sign_out(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket) {
        socket.emit("user_signed_out", JSON.stringify(serverHandler.signOut(io, socket)));
    }

    @OnMessage("update_profile")
    public async update_profile(@SocketIO() io: SocketIO.Server, @ConnectedSocket() socket: SocketIO.Socket, @MessageBody() profile: PrivateProfile) {
        socket.emit("profile_updated", JSON.stringify(await serverHandler.updateProfile(io, socket, profile)));
    }

    // @OnMessage("get_users_outside_room")
    // public get_rooms(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
    //     socket.emit("users_outside_room", JSON.stringify(serverHandler.getUsersOutsideRoom(roomId)));
    // }
    // @OnMessage("getroom_test")
    // public async getroom_test(@ConnectedSocket() socket: SocketIO.Socket, @MessageBody() roomId: string) {
    //     socket.emit("getroom_test_res", JSON.stringify(await roomDB.getRoom(roomId)));
    // }
}
// Ref : https://www.npmjs.com/package/socket-controllers?activeTab=readme