import { Room } from "./room";

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
    isPrivate: boolean | null
}

export interface CreateMatchFeedback {
    feedback: Feedback
    matchId: string
}

export interface StartMatchFeedback {
    feedback: Feedback
    nbRounds: number
}

export enum SignInStatus {
    SignIn = "You are signed in.",
    InvalidUsername = "This username does not exist.",
    InvalidPassword = "The password is incorrect.",
    AlreadyConnected = "This user is already connected."
}

export enum SignOutStatus {
    SignedOut = "You are signed out.",
    Error = "This user is already disconnected or does not exist."
}

export enum CreateRoomStatus {
    Create = "You created and joined the room.",
    AlreadyCreated = "This room is already created.",
    InvalidUser = "You are not signed in.",
    Error = "Unexpected error."
}

export enum DeleteRoomStatus {
    Delete = "You deleted the room.",
    LeaveAndDelete = "You left and deleted the room.",
    DeleteGeneral = "You can not delete General.",
    NotEmpty = "There is someone else in the room.",
    InvalidUser = "You are not signed in.",
    InvalidRoom = "This room does not exist.",
    Error = "Unexpected error."
}

export enum JoinRoomStatus {
    Join = "You joined the room.",
    InvalidUser = "You are not signed in.",
    InvalidRoom = "This room does not exist.",
    AlreadyJoined = "You already are in this room."
}

export enum LeaveRoomStatus {
    Leave = "You left the room.",
    LeaveAndDelete = "You left and deleted the room.",
    General = "You can not leave General.",
    InvalidUser = "You are not signed in.",
    InvalidRoom = "This room does not exist.",
    NeverJoined = "You are not in this room."
}

export enum UpdateProfileStatus {
    Update = "The profile has been updated.",
    InvalidProfile = "This profile is disconnected or does not exist.",
    UnexpectedError = "Unexpected error.",
    InvalidUsername = "You can not update the username."
}

export enum InviteStatus {
    Invite = "Invite sent",
    ReceiverInRoom = "The user you are trying to invite is already in the room.",
    InvalidReceiver = "This user does not exist.",
    SenderOutRoom = "You can not send an invite in a room you did not join.",
    InvalidSender = "You are not signed in.",
    InvalidRoom = "This room does not exist."
}