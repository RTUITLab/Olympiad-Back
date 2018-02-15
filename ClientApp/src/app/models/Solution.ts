import { SolutionStatus } from './SolutionStatus';

export class Solution {
    Language: string;
    Id: string;
    Raw: string;
    ExerciseId: string;
    Status: SolutionStatus;
    Time: string;

    static prettyTime(time: string): string {
        const date = new Date(time);
        let prettyDate = '';
        prettyDate += `${Solution.round(date.getDate())}`;
        prettyDate += `.${Solution.round(date.getMonth() + 1)}`;
        prettyDate += `.${date.getFullYear()} ${date.getHours()}`;
        prettyDate += `:${Solution.round(date.getMinutes())}`;
        return  prettyDate;
    }

    static round(val: number): string {
        if (val < 10) {
            return '0' + val;
        } else {
            return val.toString();
        }
    }
}
