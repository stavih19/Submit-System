import { Exercise } from "./exercise";
import { HelpFile } from "./help-file";

export interface NewExe{
    Exercise: Exercise;
    MainDate: Date;
    HelpFiles: HelpFile[];
}