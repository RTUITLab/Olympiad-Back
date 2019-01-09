export class Helpers {
    public static prettyTime(time: string): string {
        const date = new Date(time);
        let prettyDate = '';
        prettyDate += `${Helpers.round(date.getDate())}`;
        prettyDate += `.${Helpers.round(date.getMonth() + 1)}`;
        prettyDate += `.${date.getFullYear()} ${date.getHours()}`;
        prettyDate += `:${Helpers.round(date.getMinutes())}`;
        prettyDate += `:${Helpers.round(date.getSeconds ())}`;
        return  prettyDate;
    }

    private static round(val: number): string {
        if (val < 10) {
            return '0' + val;
        } else {
            return val.toString();
        }
    }
}
