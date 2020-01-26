import { Container } from "inversify";
import { Server } from "./server";
import Types from "./types";

const container: Container = new Container();

container.bind(Types.Server).to(Server);

export { container };
