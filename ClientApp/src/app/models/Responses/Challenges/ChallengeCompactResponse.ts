import { ChallengeAccessType } from '../../General/ChallengeAccessType';


export class ChallengeCompactResponse {
    public Id: string;
    public Name: string;
    public EndTime?: string;
    public ChallengeAccessType: ChallengeAccessType;
}
