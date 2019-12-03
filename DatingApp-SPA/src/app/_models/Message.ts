export interface Message {
    id: number;
    senderId: number;
    senderFirstName: string;
    senderInsertion: string;
    senderLastName: string;
    senderMainPhotoUrl: string;
    recipientId: number;
    recipientFirstName: string;
    recipientInsertion: string;
    recipientLastName: string;
    recipientMainPhotoUrl: string;
    content: string;
    isRead: boolean;
    dateRead: Date;
    dateSend: Date;
}
