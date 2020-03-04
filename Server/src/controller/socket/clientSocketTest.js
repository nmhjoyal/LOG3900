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
    socket1.emit("sign_in", { username : "zak2", password: "banane" });
});

socket1.on("user_signed_in", function (data) {
    console.log("signed in : " + data);

    socket1.emit("create_chat_room", createroom);
    
    // socket1.emit("getroom_test", "room99");
    // socket1.emit("create_chat_room", "room1");
    // socket1.emit("join_chat_room", "room1");
    // socket1.emit("leave_chat_room", "room1");
    // socket1.emit("send_message", JSON.stringify(message));

    /*
    socket1.emit("update_profile", JSON.stringify({
        firstname: "rahal",
        lastname: "zakari",
        password: "banane",
        username: "zak3",
        avatar: "avatar3333",
        rooms_joined: []
    }));
    */
});

socket1.on("user_sent_invite", function(data) {
    console.log("sent invite : " + data);
});

socket1.on("receive_invite", function(data) {
    console.log("received invite : " + data);
});

socket1.on("get_res", function(data) {
    console.log("*" + data);
});

socket1.on("room_created", function(data) {
    console.log("room created : " + data);
    socket1.emit("send_invite", invitation);
});

socket1.on("room_deleted", function(data) {
    console.log("room deleted : " + data);
});

socket1.on("user_joined_room", function(data) {
    console.log("joined room : " + data);
});

socket1.on("user_left_room", function(data) {
    console.log("left room : " + data);
});

socket1.on("new_message", function(data) {
    // console.log("new message :" + data);
    console.log("new message : " + JSON.parse(data).content + " in " +  JSON.parse(data).roomId + " *by* " + JSON.parse(data).username);
});

socket1.on("user_signed_out", function(data) {
    console.log("signed out : " + data);
});

socket1.on("load_history", function(data) {
    console.log("load history : "  + data);
});

socket1.on("getroom_test_res", function(data) {
    console.log("room : " + data);
});

socket1.on("avatar_update", function(data) {
    console.log("avatar update : " + data);
});

socket1.on("profile_updated", function(data) {
    console.log("profile updated  : " + data);
});