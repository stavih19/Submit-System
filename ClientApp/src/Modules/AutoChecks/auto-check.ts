import { AdvencedConfiguration } from "./advenced-configuration";

export interface AutoCheck {
    id: number,
    weight: number,
    input: string,
    expectedOutput: string,
    outputFileName: string,
    argumentsString: string,
    timeoutInSeconds: number,
    mainSourseFile: string,
    adittionalFilesLocation: string[],
    exerciseID: string,
    type: number,
    additionFileNames: string[],
    has_Adittional_Files: boolean
}