import {Uuid} from 'uuid';

export class Exercise {
    constructor(
        public ExerciseName?: string,
        public ExerciseID?: Uuid,
        public ExerciseTask?: string,
        public Score?: number
    ) {}
}
