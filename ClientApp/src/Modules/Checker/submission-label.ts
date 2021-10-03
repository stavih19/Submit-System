import { UserLabel } from "../Teacher/user-label";

export interface SubmissionLabel {
    id: string;
    state: number;
    submitters: UserLabel[];
    checkState: number;
    currentChecker: string;
}