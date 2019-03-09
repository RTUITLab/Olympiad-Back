import { Challenge } from '../models/Responses/Challenges/Challenge';
import { ChallengeState } from '../models/General/ChallengeState';
import { DateHelpers } from './DateHelpers';

export class ChallengeHelpers {

    public static ChallengeTime(challenge: Challenge): string {
        const state = ChallengeHelpers.CalcChallengeState(challenge);
        switch (state) {
            case ChallengeState.NoLimits:
                return 'Бессрочное соревнование';
            case ChallengeState.Ended:
                return 'Соревнование закончилось';
            case ChallengeState.NotStarted:
                return `До начала: ${DateHelpers.difference(new Date(), new Date(challenge.StartTime))}`;
            case ChallengeState.InProgress:
                return `До конца соревнования: ${DateHelpers.difference(new Date(), new Date(challenge.EndTime))}`;
            case ChallengeState.IncorrectLimits:
            default:
                return 'Неверные данные о соревновании, обратитесь к администратору';
        }
    }


    public static CalcChallengeState(challenge: Challenge): ChallengeState {
        if (!challenge) {
            return ChallengeState.IncorrectLimits;
        }
        if (!challenge.StartTime || !challenge.EndTime) {
            return ChallengeState.NoLimits;
        }

        const now = new Date();
        const nowTime = now.getTime();

        const start = new Date(challenge.StartTime);
        const startTime = start.getTime();

        const end = new Date(challenge.EndTime);
        const endTime = end.getTime();

        if (startTime > nowTime) {
            return ChallengeState.NotStarted;
        }
        if (nowTime > endTime) {
            return ChallengeState.Ended;
        }
        if (startTime <= nowTime && nowTime <= endTime) {
            return ChallengeState.InProgress;
        }
        return ChallengeState.IncorrectLimits;
    }
}
