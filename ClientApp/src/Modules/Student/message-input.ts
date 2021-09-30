import { SubmitFile } from "../Teacher/submit-file";

export interface MessageInput {
    chatID: string;
    text: string;
    attachedFile: SubmitFile;
    filePath: string;
}