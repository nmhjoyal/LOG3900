export interface Feedback {
    status: boolean
    log_message: string
}

export interface SignInFeedback {
    feedback: Feedback
    rooms_joined: string[]
}

export enum SignInStatus {
    SignIn = "You are signed in!",
    InvalidUsername = "This username does not exist",
    InvalidPassword = "The password is incorrect",
    AlreadyConnected = "This user is already connected"
}

export enum SignOutStatus {
    SignedOut = "You are signed out",
    Error = "This user is already disconnected or does not exist"
}

export enum CreateRoomStatus {
    Create = "You created and joined the room!",
    AlreadyCreated = "This room is already created",
    UserNotConnected = "The user is not connected"
}

export enum JoinRoomStatus {
    Join = "You joined the room!",
    InvalidRoom = "This room does not exist",
    AlreadyJoined = "You already are in this room"
}

export enum LeaveRoomStatus {
    Leave = "You left the room!",
    InvalidRoom = "This room does not exist",
    NeverJoined = "You are not in this room"
}