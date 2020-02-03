import * as bodyParser from "body-parser";
import * as express from "express";
import { Application } from "express";
import * as http from "http";
import { injectable } from "inversify";
import * as morgan from "morgan";
import "reflect-metadata";
import { useExpressServer } from "routing-controllers";
import { HttpController } from "./controller/http/httpController";
import * as ServerConfig from "./serverConfig.json";
import { useSocketServer } from "socket-controllers";
import { SocketProtoController } from "./controller/socket/socketController"
import * as socketioImport from "socket.io"

@injectable()
export class Server {
    private server: http.Server;
    private app: Application;

    public constructor() {
        this.app = express();
        this.app.use(bodyParser.json({limit: "50mb"}));
        this.app.use(bodyParser.urlencoded({limit: "50mb", extended: true, parameterLimit: 50000}));
        this.app.use(morgan("dev"));
    }

    public init(): void {
        let port: number = ServerConfig.port;

        this.app.set("port", port);
        this.server = http.createServer(this.app);
        this.server.on("error", (error) => console.log(error));

        // Start socket 
        const io: SocketIO.Server = socketioImport(this.server);
        useSocketServer(io, {
            controllers: [ SocketProtoController ]
        });

        // Start http server
        useExpressServer(this.app, {
            controllers: [ HttpController ] 
        });
        
        this.server.listen(port);
        console.log("Listening on port " + port + " ...");
    }
}
