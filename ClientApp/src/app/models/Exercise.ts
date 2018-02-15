import {Guid} from 'guid';

export class Exercise {
    constructor(
        public ExerciseName?: string,
        public ExerciseID?: Guid,
        public ExerciseTask?: string,
        public Score?: number
    ) {}
}
