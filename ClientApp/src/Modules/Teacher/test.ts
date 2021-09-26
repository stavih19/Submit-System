export interface Test {
    id: number;
    type: number;
    exerciseID: string;
    weight: number;
    input: string;
    expectedOutput: string;
    outputFileName: string;
    argumentsString: string;
    timeoutInSeconds: number;
    mainSourseFile: string;
    adittionalFilesLocation: string;
    additionFileNames: string[];
    has_Adittional_Files: boolean;
}