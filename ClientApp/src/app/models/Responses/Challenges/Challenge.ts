import { ChallengeAccessType } from '../../General/ChallengeAccessType';
import { ExerciseListResponse } from '../ExerciseListResponse';


export class Challenge {
    public Id: string;
    public Name: string;
    public CreationTime: string;
    public StartTime?: string;
    public EndTime?: string;
    public ChallengeAccessType: ChallengeAccessType;
}
