import { SolutionStatus } from './SolutionStatus';

export class Solution {
    Language: string;
    Id: string;
    Raw: string;
    ExerciseId: string;
    Status: SolutionStatus;
    Time: string;

    static prettyTime(time: string) {
        const date = new Date(time);
        return `${Solution.round(date.getDate())}.${Solution.round(date.getMonth() + 1)}.${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`;
    }

    static round(val: number): string {
        if (val < 10) {
            return '0' + val;
        } else {
            return val.toString();
        }
    }
}
