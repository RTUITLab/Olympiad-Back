import { SolutionStatus } from '../SolutionStatus';

export class SolutionStatusConverter {

    private static readonly info: StatusDesc[] = [
        {
            status: SolutionStatus.InQueue,
            pretty: 'В очереди на проверку'
        },
        {
            status: SolutionStatus.InProcessing,
            pretty: 'Проверяется'
        },
        {
            status: SolutionStatus.CompileError,
            pretty: 'Ошибка компиляции'
        },
        {
            status: SolutionStatus.TooLongWork,
            pretty: 'Программа работает слишком долго'
        },
        {
            status: SolutionStatus.RunTimeError,
            pretty: 'Ошибка во время исполнения'
        },
        {
            status: SolutionStatus.Sucessful,
            pretty: 'Задание решено верно'
        },
        {
            status: SolutionStatus.WrongAnswer,
            pretty: 'Программа выдает неверный ответ'
        }
    ];
    static convertToPretty(status: SolutionStatus): string {
        return SolutionStatusConverter.info.find(D => D.status === status).pretty;
    }
}
class StatusDesc {
    status: SolutionStatus;
    pretty: string;
}
