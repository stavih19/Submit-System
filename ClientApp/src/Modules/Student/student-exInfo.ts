import { Chat } from "../chat";
import { SubmitDate } from "../Reduce/submit-date";
import { UserLabel } from "../Teacher/user-label";

export class StudentExInfo {
    totalGrade: number;
    submissionID: string;
    extensionChat: Chat;
    appealChat: Chat;
    maxSubmitters: number;
    submitters: UserLabel[];
    exID: string;
    exName: string;
    dateSubmitted: Date;
    dates: SubmitDate[];
    isMultipleSubmission: boolean;
    manualCheckInfo: string;
    submissionFolder: string;
    filenames: string[];
    state: number;
    isMainSubmitter: boolean;
}