import { SolutionStatus } from './SolutionStatus';

export class Solution {
    Language: string;
    Id: string;
    Raw: string;
    ExerciseId: string;
    Status: SolutionStatus;
    SendingTime: string;
    StartCheckingTime: string;
    CheckedTime: string;
}
