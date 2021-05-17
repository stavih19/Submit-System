import { Chat } from "./Chat";
import { Student } from "./Student";
import { SubmitDate } from "./SubmitDate";

export class StudentExInfo {
    submissionID: string;
    totalGrade: number;
    autoGrade: number;
    styleGrade: number;
    manualGrade: number;
    extensionChat: Chat;
    appealChat: Chat;
    maxSubmitters: number;
    submitters: Student[];
    exID: string;
    exName: string;
    dates: SubmitDate[];
    dateSubmitted: Date;
    isMultipleSubmission: boolean;
}