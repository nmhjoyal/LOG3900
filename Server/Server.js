const http = require('http');
const server = http.createServer((req, res) => {
    console.log("req : " + req);
    if(req.url === '/'){
        res.write("Hi");
        res.end();
    }
});

server.on('connection', (socket) => {
    console.log(socket);
});

server.listen(5000);

/*
socket.on('data',function(data){
    var bread = socket.bytesRead;
    var bwrite = socket.bytesWritten;
    console.log('Bytes read : ' + bread);
    console.log('Bytes written : ' + bwrite);
    console.log('Data sent to server : ' + data);
  
    //echo data
    var is_kernel_buffer_full = socket.write('Data ::' + data);
    if(is_kernel_buffer_full){
      console.log('Data was flushed successfully from kernel buffer i.e written successfully!');
    }else{
      socket.pause();
    }
});
*/