import * as bodyParser from "body-parser";
import * as express from "express";
import { Application } from "express";
import * as http from "http";
import { injectable } from "inversify";
import * as morgan from "morgan";
import "reflect-metadata";
import { useExpressServer } from "routing-controllers";
import { UserController } from "./controllers/userController";
import * as ServerConfig from "./serverConfig.json";


@injectable()
export class Server {
    private server: http.Server;
    private app: Application;

    public constructor() {
        this.app = express();
        this.app.use(bodyParser.json({limit: "50mb"}));
        this.app.use(bodyParser.urlencoded({limit: "50mb", extended: true, parameterLimit:50000}));
        this.app.use(morgan("dev"));
    }

    public init(): void {
        this.app.set("port", ServerConfig.port);
        this.server = http.createServer(this.app);
        this.server.on("error", (error) => console.log(error));

        useExpressServer(this.app, {
            controllers: [
                UserController
            ] 
        });
        
        this.server.listen(ServerConfig.port);
        console.log("Listening ... ");
    }
}