import Room from "./room";

export interface Feedback {
    status: boolean
    log_message: string
}

export interface SignInFeedback {
    feedback: Feedback
    // Contains histories
    rooms_joined: Room[]
}

export interface JoinRoomFeedback {
    feedback: Feedback
    // Contains history
    room_joined: Room | null
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

export enum DeleteRoomStatus {
    Delete = "You deleted the room",
    LeaveAndDelete = "You left and deleted the room",
    DeleteGeneral = "You can not delete General",
    NotEmpty = "There is someone else in the room",
    InvalidRoom = "This room does not exist"
}

export enum JoinRoomStatus {
    Join = "You joined the room!",
    InvalidRoom = "This room does not exist",
    AlreadyJoined = "You already are in this room"
}

export enum LeaveRoomStatus {
    Leave = "You left the room!",
    General = "You can not leave General",
    InvalidRoom = "This room does not exist",
    NeverJoined = "You are not in this room"
}