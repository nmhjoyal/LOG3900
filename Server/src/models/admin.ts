import Message from "./message";

export default class Admin {
    public static readonly admin: string = "Admin";

    public static createAdminMessage(content: string, roomId: string): Message {
        const message: Message = {
            author : {
                username: this.admin, 
                avatar: this.admin
            },
            content : content,
            date : Date.now(),
            roomId : roomId
        };
        return message;
    }
}


