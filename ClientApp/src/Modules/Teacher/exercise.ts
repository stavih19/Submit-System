export interface Exercise {
    id: string;
    name: string;
    courseID: string;
    originalExerciseID: string;
    maxSubmitters: number;
    filesLocation: string;
    reductions: number[];
    lateSubmissionSettings: string;
    programmingLanguage: string;
    autoTestGradeWeight: number;
    styleTestGradeWeight: number;
    isActive: number;
    multipleSubmission: boolean;
    mossShownMatches: number;
    mossMaxTimesMatch: number;
    mossLink: string;
    filenames: string[];
}