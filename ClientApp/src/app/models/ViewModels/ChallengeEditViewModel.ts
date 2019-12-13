import { ChallengeAccessType } from '../General/ChallengeAccessType';

export class ChallengeEditViewModel {
    public Name: string;
    public StartTime: string;
    public EndTime: string;
    public ChallengeAccessType: ChallengeAccessType;
    public AddPersons: string[];
    public RemovePersons: string[];
}
