import { SolutionStatus } from '../SolutionStatus';


export class ExerciseListResponse {
    constructor(
        public Name?: string,
        public Id?: string,
        public Score?: number,
        public Status?: SolutionStatus
    ) {}
}
