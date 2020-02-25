//client.js
var io = require('socket.io-client');
var socket1 = io.connect('http://localhost:5000', { reconnect: true});


socket1.on("connect", function (data) {
    socket1.emit("sign_in", { username : "yi", password: "test123" })
});

socket1.on("user_signed_in", function (data) {
    console.log(data);
    // console.log("user is signed in!")
});