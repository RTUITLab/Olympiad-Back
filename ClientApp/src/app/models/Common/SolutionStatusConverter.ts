import { SolutionStatus } from '../SolutionStatus';

export class SolutionStatusConverter {
    static convert(status: SolutionStatus): string {
        switch (status) {
            case SolutionStatus.InQueue:
                return 'В очереди на проверку';
            case SolutionStatus.InProcessing:
                return 'Проверяется';
            case SolutionStatus.CompileError:
                return 'Ошибка компиляции';
            case SolutionStatus.RunTimeError:
                return 'Ошибка во время исполнения';
            case SolutionStatus.Sucessful:
                return 'Задание решено верно';
            case SolutionStatus.WrongAnswer:
                return 'Программа выдает неверный ответ';
        }
    }
}
