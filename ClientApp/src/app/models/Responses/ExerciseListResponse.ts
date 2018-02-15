import {Guid} from 'guid';
import { SolutionStatus } from '../SolutionStatus';

export class ExerciseListResponse {
    constructor(
        public Name?: string,
        public Id?: Guid,
        public Score?: number,
        public Status?: SolutionStatus
    ) {}
}
