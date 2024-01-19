C#
Для языка `C#` используется .Net Core 3.1.

# Сборка
```bash
dotnet build /src/src.csproj -o /pub -c Release --no-dependencies --no-restore
```

# Запуск
```bash
dotnet /pub/src.dll
```