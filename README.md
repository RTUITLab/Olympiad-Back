# Student Testing System
## Description
System provides examination testing for students on the basis of the given test tasks.

Student choses a task he wants to complete and reads the terms.

He develops the program according to the task using one of the available programming languages.

After sending the solution the System executes the code and inputs test datasets. For being solved correctly the program must output the correct results for all test inputs.

## How to run:

### WebApp 
Represents the System BackEnd storing all the necessary data.

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
        "IsRegisterAvailable": true|false // if false - new users can be added only via admin panel
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
    "USE_DEBUG_EMAIL_SENDER": true|false // if true email messages will be printed to console
    "USE_DEBUG_RECAPTCHA_VERIFIER": true|false //if true recaptcha always will be valid,
    "USE_CHECKING_RESTART": true|false // if true, solutions the check of which has started more than 2 minutes ago would be placed in the queue again.
}
```

You must use connection string to your real Postgres database in the field  ```ConnectionStrings:PostgresDataBase```, for example:
> _User ID=postgres;Password=password;Server=127.0.0.1;Port=5432;Database=TestDBdotnet;Integrated Security=true;_ 


3. Use ```dotnet run``` to start web app in Development environment (appsettings.Development.json will be used)

### Client App

Represents the client part on Angular

Install the following:
1. Node.js + NPM

How to run:

1. Use ```npm instal``` in _./ClientApp_ folder. It will install all the dependecies in __node_modules__ folder.

2. Create the _environment.ts_ in _./ClientApp/src/environments_ folder. Fill it with the following:

```json
export const environment = {
    production: true,
    isAdmin: false,
    baseUrl: '',
    buildNumber: '',
    showResults: true,
    recaptchaClientToken: ''
};
```
> baseUrl - url of running WebApp, when running locally it will most likely be ```http://localhost:64800```
 
> recaptchaClientToken - **PUBLIC** token got when creating reCAPCTCHA v2

2. After running the site, you must execute ```npm start```. After this you can see the site in your browser on [localhost:4200](http://localhost:4200).

### Executor

Application connected to WebApp which is executing received solutions and checks them.

You must have **Docker** installed on the system you want to run Executor on. Actually **Docker** executes the received programs.


1. Create _./Executor/appsettings.Secret.json_

2. Fill it with the following:

```js
{
    "StartSettings": {
        "Address":"http://localhost:5000", // Address of olympiad webapp
        "DockerEndPoint":"http://localhost:2375" // Address of docker endpoint
    },
    "UserInfo": {
        "UserName":"Username having Executor rights",
        "Password":"User password"
    }
}
```

3. After creating the file you can execute ```dotnet run``` in _./Executor_ folder.