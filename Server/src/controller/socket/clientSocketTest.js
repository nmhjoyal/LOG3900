//client.js
var io = require('socket.io-client');
var socket1 = io.connect('http://10.200.28.27:5000', { reconnect: true});

socket1.on("connect", function (data) {
    socket1.emit("sign_in", { username : "hub", password: "123" })
});

socket1.on("user_signed_in", function (data) {
    console.log("user is signed in!")
    socket1.emit("send_message", {author:{userame: "hub", password:"124"}, content:"lawkjdawldkj"})
});