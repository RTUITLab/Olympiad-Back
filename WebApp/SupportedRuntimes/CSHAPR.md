csharp
C#
.cs
Для языка `C#` используется .Net 8.0.

## Пример
```csharp
Console.WriteLine("Hello, World!");
```

# Сборка
```bash
dotnet build /src/src.csproj -o /pub -c Release --no-dependencies --no-restore
```

# Запуск
```bash
dotnet /pub/src.dll
```