using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    internal class UsersGenerateModelBuilder
    {
        public string SourceFileName { get; }
        public IReadOnlyList<string> ColumnNames { get; }

        private readonly bool containsDefaultPassword;
        private int SkipColumnsCount => containsDefaultPassword ? 3 : 2; // ID, Name, DefaultPassword?

        public const string IDColumnName = "ID";
        public const string NameColumnName = "Name";
        public const string DefaultPassworkColumnName = "DefaultPassword";

        private readonly List<UserGenerateRow> userGenerateRows = new();
        public UsersGenerateModelBuilder(string sourceFileName, string[] columnNames)
        {
            SourceFileName = sourceFileName;
            if (columnNames.Length < 2)
            {
                throw new IncorrectUsersFileFormatException("Обязательные колонки", "Первые две колонки 'ID' и 'Name' обязательны");
            }
            if (columnNames[0] != IDColumnName)
            {
                throw new IncorrectUsersFileFormatException("Название первой колонки", "Первая колонка должна называться 'ID'");
            }
            if (columnNames[1] != NameColumnName)
            {
                throw new IncorrectUsersFileFormatException("Название второй колонки", "Вторая колонка должна называться 'Name'");
            }
            for (int i = 2; i < columnNames.Length; i++)
            {
                if (!Regex.IsMatch(columnNames[i], @"^\S+$"))
                {
                    throw new IncorrectUsersFileFormatException(columnNames[i], $"Название свойства должно состоять из непробельных символов");
                }
            }
            if (columnNames.Length > 2 && columnNames[2] == DefaultPassworkColumnName)
            {
                containsDefaultPassword = true;
            }
            ColumnNames = columnNames.Skip(SkipColumnsCount).ToArray();
        }

        public void AddUserRow(string[] row, Func<string> createPasswordFunc)
        {
            if (row.Length - SkipColumnsCount != ColumnNames.Count)
            {
                throw new Exception("Incorrect row length");
            }
            var userPassword = containsDefaultPassword ? row[2] : createPasswordFunc();
            if (userPassword is null)
            {
                throw new IncorrectUsersFileFormatException(
                    "Некорректный пароль",
                    $"Для пользователя {row[0]} задан пароль '{userPassword}', являющийся некорректным.");
            }
            if (userPassword.Length < 6)
            {
                throw new IncorrectUsersFileFormatException(
                    "Слишком короткий пароль",
                    $"Для пользователя {row[0]} задан пароль '{userPassword}' длиной {userPassword.Length}. Минимальная длина пароля - 6 символов.");
            }
            userGenerateRows.Add(new UserGenerateRow
            (
                StudentID: row[0],
                FirstName: row[1],
                Password: userPassword,
                Claims: row
                    .Skip(SkipColumnsCount)
                    .Select((r, i) => new System.Security.Claims.Claim(ColumnNames[i], r))
                    .ToList()
            ));
        }

        public UsersGenerateModel Build()
        {
            if (userGenerateRows.Count == 0)
            {
                throw new IncorrectUsersFileFormatException("Пустой файл", "В выбранном файле нет записей о пользователеях");
            }
            return new UsersGenerateModel(SourceFileName, ColumnNames, userGenerateRows);
        }
    }
}
