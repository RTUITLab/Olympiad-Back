using Olympiad.Shared.Models;

namespace Olympiad.ControlPanel.Extensions
{
    public static class EnumExtensions
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
        public static string ToRussian(this ChallengeAccessType challengeAccessType) => challengeAccessType switch
        {
            ChallengeAccessType.Private => "По приглашениям",
            ChallengeAccessType.Public => "Публичное",
            _ => "Некорректное значение"
        };

        public static string IconType(this ChallengeAccessType challengeAccessType) => challengeAccessType switch
        {
            ChallengeAccessType.Private => AntDesign.IconType.Outline.EyeInvisible,
            ChallengeAccessType.Public => AntDesign.IconType.Outline.Eye,
            _ => AntDesign.IconType.Outline.Warning
        };
    }
}
