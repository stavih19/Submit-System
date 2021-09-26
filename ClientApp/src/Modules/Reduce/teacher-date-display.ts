import { UserLabel } from "../Teacher/user-label";

export interface TeacherDateDisplay {
    id: number;
    dateTime: Date;
    group: number;
    reduction: number;
    submitters: UserLabel[];
}