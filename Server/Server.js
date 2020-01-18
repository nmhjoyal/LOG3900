const http = require('http');
const server = http.createServer();

server.on('connection', (socket) => {
    console.log('New connection');
});

server.listen(5000);