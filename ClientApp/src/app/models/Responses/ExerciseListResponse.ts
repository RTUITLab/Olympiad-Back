import {Uuid} from 'uuid';
import { SolutionStatus } from '../SolutionStatus';

export class ExerciseListResponse {
    constructor(
        public Name?: string,
        public Id?: Uuid,
        public Score?: number,
        public Status?: SolutionStatus
    ) {}
}
