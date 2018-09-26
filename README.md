# Система для тестирования студентов

## Описание
Система предназначена для проведения тестирования студентов, на основе заданных тестовых задач

Студент, попадая в систему выбирает задачу, которую он хочет решить, и читает условие задачи.

Затем он пишет решение на одном из представленных ЯП

Послке отправки кода система выполняет этот код, и подает на вход тестовые наборы данных, для зачтения задания программа должна выдать правильный результат на всех наборах

## Как запустить:

### WebApp 
Представляет BackEnd системыв, хранящий все необходимые сведения

1. Создать файл \WebApp\appsettings.Secret.json

2. Заполнить файл следующим содержанием

```json
{
    "JwtIssuerOptions": {
        "SecretKey":"Любая случайная строка, желатьельно длиной около 30-и символов"
    },
    "ConnectionStrings": {
        "RemoteDB": "Строка подключения к SqlServer"
    }
}
```

Для поля **RemoteDB** можно использовать строку подключения к реальному удаленному SQL Server, например 
> _Server=server_host,1433;Initial Catalog=database_name;Persist Security Info=False;User ID=user_login;Password=user_password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;_ 

Или использовать локальную базу данных, например имея строку 
> _Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=OlympiadDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False_

3. После настройки БД необходимо применить миграцию к существующей пустой БД, для этого необходимо в папке WebApp выполнить команду ```dotnet ef database update```. эта команда создаст необходимую базу данных, с необходимыми таблицами.

4. После этого можно запускать приложение, при помощи команды ```dotnet run INIT_ROLES=true```

### Client App

Представляет клиентсую часть на Angular

Необходимо установить:
1. Node.js + NPM

Запуск:

1. В папке /ClientApp выполнить команду ```npm instal``` Эта команда установит все зависимости в папку __node_modules__.

2. 