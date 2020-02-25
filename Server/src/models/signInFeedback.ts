export default interface SignInFeedback {
    status: boolean,
    log: ConnectionStatus
}

export enum ConnectionStatus {
    Connect = "",
    InvalidUsername = "This username does not exist",
    InvalidPassword = "This username does not exist",
    AlreadyConnected = "This user is already connected"
}
