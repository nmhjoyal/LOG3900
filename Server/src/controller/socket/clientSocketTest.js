//client.js
var io = require('socket.io-client');
var socket = io.connect('http://localhost:5000', { reconnect: true});

// socket.connect();
socket.emit("test");

