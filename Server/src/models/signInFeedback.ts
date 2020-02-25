export default interface SignInFeedback {
    signed_in: boolean,
    log_message: ConnectionStatus
}

export enum ConnectionStatus {
    Connect = "You are signed in!",
    InvalidUsername = "This username does not exist",
    InvalidPassword = "The password is incorrect",
    AlreadyConnected = "This user is already connected"
}
