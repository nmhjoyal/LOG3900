//client.js
var io = require('socket.io-client');
var socket1 = io.connect('http://localhost:5000', { reconnect: true });

var message = {
    content : "Fuck you, va chier",
    roomId: "room1"
}

var invitation = {
    id: "room",
    username: "zak1"
}

var createroom = {
    id: "room",
    isPrivate: true
}

socket1.on("connect", function (data) {
    socket1.emit("potrace");
});

socket1.on("sent_path", function(data) {console.log("ICI !!!!! : " + data)});

socket1.on("user_signed_in", function (data) {
    console.log("signed in : " + data);
    
    socket1.emit("connect_free_draw");
    // socket1.emit("create_chat_room", createroom);
    // socket1.emit("getroom_test", "room99");
    // socket1.emit("create_chat_room", "room1");
    // socket1.emit("join_chat_room", "test");
    // socket1.emit("leave_chat_room", "room1");
    // socket1.emit("send_message", JSON.stringify(message));
    // setTimeout(function(){socket1.emit("disconnect_free_draw");} , 20000);

});
socket1.on("path_is_sent", function() {console.log("You sent the drawing");});
socket1.on("drawer", function() {console.log("You are the drawer!");});
socket1.on("observer", function() {console.log("You are the observer!");});

socket1.on("user_sent_invite", function(data) {console.log("sent invite : " + data);});
socket1.on("receive_invite", function(data) { console.log("received invite : " + data);});
socket1.on("get_res", function(data) { console.log("*" + data);});
socket1.on("room_created", function(data) { 
    console.log("room created : " + data);
    socket1.emit("update_profile", JSON.stringify({
        username: "hub1",
        firstname: "hubert",
        lastname: "m-d",
        password: "banane",
        avatar: "newAvatar1",
        rooms_joined: []
    }));
});
socket1.on("room_deleted", function(data) { console.log("room deleted : " + data);});
socket1.on("user_joined_room", function(data) { console.log("joined room : " + data);});
socket1.on("user_left_room", function(data) {console.log("left room : " + data);});
socket1.on("new_message", function(data) { console.log("new message : " + JSON.parse(data).content + " in " +  JSON.parse(data).roomId + " *by* " + JSON.parse(data).username);});
socket1.on("user_signed_out", function(data) {console.log("signed out : " + data);});
socket1.on("load_history", function(data) {console.log("load history : "  + data);});
socket1.on("getroom_test_res", function(data) {console.log("room : " + data);});
socket1.on("avatar_update", function(data) {console.log("avatar update : " + data);});
socket1.on("profile_updated", function(data) {console.log("profile updated  : " + data);});