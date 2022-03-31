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

        public static string ToRussian(this ExerciseType exerciseType) =>
            exerciseType == ExerciseType.Code ? "Програмный код" :
            exerciseType == ExerciseType.Docs ? "Документы" :
            AntDesign.IconType.Outline.FileUnknown;
        public static string IconType(this ExerciseType exerciseType) =>
            exerciseType == ExerciseType.Code ? AntDesign.IconType.Outline.Code :
            exerciseType == ExerciseType.Docs ? AntDesign.IconType.Outline.FileZip :
            AntDesign.IconType.Outline.FileUnknown;
    }
}
