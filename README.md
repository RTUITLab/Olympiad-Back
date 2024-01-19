# Student Testing System
[![Build Status](https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_apis/build/status/Olympiad/Olympiad-Back?branchName=master)](https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_build/latest?definitionId=125&branchName=master)
[![master tests](https://img.shields.io/azure-devops/tests/RTUITLab/RTU%20IT%20Lab/125/master?label=tests&style=plastic)](https://dev.azure.com/rtuitlab/RTU%20IT%20Lab/_build/latest?definitionId=125&branchName=master)
## Description
System provides examination testing for students on the basis of the given test tasks.

Student choses a task he wants to complete and reads the terms.

He writes a program according to the task using one of available programming languages.

After sending a solution System executes the code and inputs test datasets. To consider solved correctly, the program must return correct results for all test inputs.

## How to run:

### WebApp 
Represents System BackEnd storing all the necessary data.

First, install .Net Core 3.1


1. Create _./WebApp/appsettings.Secret.json_ file

2. Fill _./WebApp/appsettings.Secret.json_ with the following: 

```js
{
    "JwtIssuerOptions": {
        "SecretKey":"Random string nearly 30 symbols"
    },
    "ConnectionStrings": {
        "PostgresDataBase": "The connection string to the Postgres database"
    },
    "EmailSettings": {
        "Email": "Email of sender account",
        "Password": "Password of sender account",
        "SmtpHost": "smtp.host",
        "SmtpPort": "smtp.port",
        "SmtpUseSsl": true|false
	},
    "AccountSettings": {
        "IsRegisterAvailable": true|false // if false - new users can be added only via control panel
	},
    "DefaultUserSettings": {
        "Email": "email of user, which can be created",
        "Password": "password of user, which can be created",
        "Name": "name of user, which can be created",
        "StudentId": "student id of user, which be created",
        "CreateUser": true|false // if true - user with parameters, written above will be created,
        "Roles": [ // All roles will be applyed to account with written email, ignoring CreateUser field
            "Admin",
            "User",
            "Executer",
            "ResultsViewer"
		]
	},
    "GenerateSettings": {
        "Domain": "domain, attached to user email while generating, '@rtuitlab.dev' for example"
	},
    "RecaptchaSettings": {
        "SecretKey": "PRIVATE token got when creating reCAPCTCHA v2"
	},
    "USE_DEBUG_EMAIL_SENDER": true|false, // if true email messages will be printed to console
    "USE_DEBUG_RECAPTCHA_VERIFIER": true|false, // if true recaptcha always will be valid
    "USE_CHECKING_RESTART": true|false // if true, solutions the check of which has started more than 2 minutes ago would be placed in the queue again
    "USE_MOCK_QUEUE": true|false // if true, mock queue service will be used. If false - rabbitmq queue
}
```

You must use connection string to your real Postgres database in the field  ```ConnectionStrings:PostgresDataBase```, for example:
> _User ID=postgres;Password=password;Server=127.0.0.1;Port=5432;Database=TestDBdotnet;_ 


3. Use ```dotnet run``` to start web app in Development environment (appsettings.Development.json will be used)
### Executor

Application connected to WebApp, which is executing received solutions and checks them.

You must have **Docker** installed on the system you want to run Executor on. Actually **Docker** executes received programs.


1. Create _./Executor/appsettings.Local.json_

2. Fill it with the following:

```js
{
    "StartSettings": {
        "Address":"http://localhost:5000", // Address of olympiad webapp
        "DockerEndPoint":"http://localhost:2375", // Address of docker endpoint
        "PrivateDockerRegistry": { // OPTIONAL, use when you reach rate limit od dockerhub pulls (100 per 6 hours)
            "Address": "host of your private registry",
            "Login": "login of registry",
            "Password": "password of registry"
        }
    },
    "UserInfo": {
        "UserName":"Username having Executor rights",
        "Password":"User password"
    },
    "RunningSettings": {
        "WorkersPerCheckCount": 5 // Every solution will be checked with that count of parallel workers (valid values - [1..100]), select value according to your machine configuration (for surface book 2 - optimal is 5-10). Too large value can break your docker.
    },
    "CONSOLE_MODE": "Logs" | "StatusReporting",// Logs - just write logs, StatusReporting - updatable status and 10 last logs with possible artifacts =
    "RabbitMqQueueSettings" :{
        "Host": "RABBIT MQ HOST",
        "QueueName": "QUEUE NAME FOR SOLUTION TASKS",
        "ClientProvidedName": "name for executor service"
    }
}
```

3. After creating the file you can execute ```dotnet run``` in _./Executor_ folder.

## How to run self hosted executor on Windows machine:

Can be run only with linux docker daemon
