using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Olympiad.ControlPanel.Pages.Users.GenerateUsers
{
    internal class UsersGenerateModel
    {
        public string SourceFileName { get; }
        public string[] ColumnNames { get; }
        public const string DefaultPassworkColumnName = "DefaultPassword";
        public IReadOnlyCollection<UserGenerateRow> UserGenerateRows => userGenerateRows;

        private List<UserGenerateRow> userGenerateRows = new List<UserGenerateRow>();

        public UsersGenerateModel(string sourceFileName, string[] columnNames)
        {
            SourceFileName = sourceFileName;
            if (columnNames.Length < 2)
            {
                throw new IncorrectColumnsException("Обязательные колонки", "Первые две колонки 'ID' и 'Name' обязательны");
            }
            if (columnNames[0] != "ID")
            {
                throw new IncorrectColumnsException("Название первой колонки", "Первая колонка должна называться 'ID'");
            }
            if (columnNames[1] != "Name")
            {
                throw new IncorrectColumnsException("Название второй колонки", "Вторая колонка должна называться 'Name'");
            }
            for (int i = 2; i < columnNames.Length; i++)
            {
                if (!Regex.IsMatch(columnNames[i], @"^\S+$"))
                {
                    throw new IncorrectColumnsException(columnNames[i], $"Название свойства должно состоять из непробельных символов");
                }
            }
            ColumnNames = columnNames;
        }

        public void AddUserRow(string[] row, Func<string> createPasswordFunc)
        {
            if (row.Length != ColumnNames.Length)
            {
                throw new Exception("Incorrect row length");
            }
            userGenerateRows.Add(new UserGenerateRow
            (
                StudentID: row[0],
                FirstName: row[1],
                Password: IsContainsPasswordColumn(out var passworcColumnNameIndex) ?
                    row[passworcColumnNameIndex] : createPasswordFunc(),
                Claims: row
                    .Skip(2)
                    .Select((r, i) => new System.Security.Claims.Claim(ColumnNames[i + 2], r))
                    .ToList()
            ));
        }

        public bool IsContainsPasswordColumn(out int index)
        {
            index = Array.IndexOf(ColumnNames, DefaultPassworkColumnName);
            return index != -1;
        }
    }
}
