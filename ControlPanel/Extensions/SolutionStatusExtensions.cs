using Olympiad.Shared.Models;

namespace Olympiad.ControlPanel.Extensions
{
    public static class SolutionStatusExtensions
    {
        public static string ToRussian(this SolutionStatus solutionStatus)
        {
            return solutionStatus switch
            {
                SolutionStatus.ErrorWhileCompile => "Ошибка системы при компиляции",
                SolutionStatus.CompileError => "Ошибка при компиляции",
                SolutionStatus.RunTimeError => "Ошибка во время исполнения",
                SolutionStatus.WrongAnswer => "Неверный ответ",
                SolutionStatus.TooLongWork => "Слишком долгое выполнение",
                SolutionStatus.InQueue => "В очереди на проверку",
                SolutionStatus.InProcessing => "Идет проверка",
                SolutionStatus.Successful => "Решено верно",
                _ => "Некорректный статус",
            };
        }
    }
}
