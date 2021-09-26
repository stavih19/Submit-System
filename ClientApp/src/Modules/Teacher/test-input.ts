import { SubmitFile } from "./submit-file";
import { Test } from "./test";

export interface TestInput {
    test: Test;
    additionalFiles: SubmitFile[];
}