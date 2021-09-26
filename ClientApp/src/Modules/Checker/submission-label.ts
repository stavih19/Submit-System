import { UserLabel } from "../Teacher/user-label";

export interface SubmissionLabel {
    is: string;
    state: number;
    submitters: UserLabel[];
    checkState: number;
    currentChecker: string;
}