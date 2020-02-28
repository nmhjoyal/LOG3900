//client.js
var io = require('socket.io-client');
var socket1 = io.connect('http://localhost:5000', { reconnect: true });
var publicProfile = {
    username : "zak2",
    avatar : "my-avatar"
}
var message = {
    author : publicProfile,
    content : "hello",
    date : Date.now(),
    roomId: "room1"
}

socket1.on("connect", function (data) {
    socket1.emit("sign_in", { username : "zak2", password: "banane" });
});

socket1.on("user_signed_in", function (data) {
    console.log("signed in : " + data);
    // socket1.emit("create_chat_room", "room3");
    socket1.emit("join_chat_room", "room1");
    // socket1.emit("join_chat_room", "room1");
    // socket1.emit("send_message", JSON.stringify(message));
});

socket1.on("room_created", function(data) {
    console.log("room created : " + data);
});

socket1.on("user_joined_room", function(data) {
    console.log("joined room : " + data);
});

socket1.on("user_left_room", function(data) {
    console.log("left room : " + data);
});

socket1.on("new_message", function(data) {
    // console.log("new message :" + data);
    console.log("new message : " + JSON.parse(data).content + " in " +  JSON.parse(data).roomId + " *by* " + JSON.parse(data).author.username);
});

socket1.on("user_signed_out", function(data) {
    console.log("signed out : " + data);
});

socket1.on("load_history", function(data) {
    console.log("load history : "  + data);
});