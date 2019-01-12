export class DateHelpers {

    private static readonly oneSecond = 1000;
    private static readonly oneMinute = DateHelpers.oneSecond * 60;
    private static readonly oneHour = DateHelpers.oneMinute * 60;
    private static readonly oneDay = DateHelpers.oneHour * 24;

    public static prettyTime(time: string): string {
        const date = new Date(time);
        let prettyDate = '';
        prettyDate += `${DateHelpers.round(date.getDate())}`;
        prettyDate += `.${DateHelpers.round(date.getMonth() + 1)}`;
        prettyDate += `.${date.getFullYear()} ${date.getHours()}`;
        prettyDate += `:${DateHelpers.round(date.getMinutes())}`;
        prettyDate += `:${DateHelpers.round(date.getSeconds())}`;
        return prettyDate;
    }

    public static difference(from: Date, to: Date): string {
        let difference = to.getTime() - from.getTime();
        let result = '';
        const days = Math.floor((difference / DateHelpers.oneDay));
        if (days > 0) {
            result += `Дней: ${days}`;
            difference -= days * DateHelpers.oneDay;
        }
        const hours = Math.floor((difference / DateHelpers.oneHour));
        if (hours > 0) {
            result += ` Часов: ${hours}`;
            difference -= hours * DateHelpers.oneHour;
        }
        const minutes = Math.floor((difference / DateHelpers.oneMinute));
        if (minutes > 0) {
            result += ` Минут: ${minutes}`;
            difference -= minutes * DateHelpers.oneMinute;
        }
        const seconds = Math.floor((difference / DateHelpers.oneSecond));
        if (seconds >= 0) {
            result += ` Секунд: ${seconds}`;
        }
        if (difference < 0) {
            result += ` Секунд: 0`;
        }
        return result;
    }


    private static round(val: number): string {
        if (val < 10) {
            return '0' + val;
        } else {
            return val.toString();
        }
    }
}
