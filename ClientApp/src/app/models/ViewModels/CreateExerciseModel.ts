export class CreateExerciseModel {
    constructor(
        public ChallengeId?: string,
        public ExerciseName?: string,
        public ExerciseTask?: string,
        public Score?: number
    ) {}
}
