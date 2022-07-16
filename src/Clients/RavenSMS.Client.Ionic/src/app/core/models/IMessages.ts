import { MessageStatus } from "../constants/enums";

/**
 * interface that defines the sms message
 */
export interface IMessages {
    /**
     * the id of the server this message is sent from.
     */
    serverId: string;

    /**
     * the id of the message.
     */
    id: string;

    /**
     * the phone number that the message is sent to.
     */
    to: string;

    /**
     * the phone number of from 
     */
    from: string;

    /**
     * the content of the message
     */
    content: string;

    /**
     * the date the message is sent
     */
    sentOn: Date | null;

    /**
     * if the message in the queue this will be the date the message will be delivered at
     */
    deliverAt: Date | null;

    /**
     * the date the message was created on
     */
    createdOn: Date;

    /**
     * the status of the message
     */
    status: MessageStatus;
}