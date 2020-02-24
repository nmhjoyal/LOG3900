export default interface SignInFeedback {
    status: boolean,
    message: ConnectionStatus
}

enum ConnectionStatus {
    InvalidUsername = "This username does not exist",
    InvalidPassword = "This username does not exist",
    UserConnected = "This user is already connected"
}
