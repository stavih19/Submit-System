import { Chat } from "./chat";
import { Student } from "./student";
import { SubmitDate } from "./submit-date";

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