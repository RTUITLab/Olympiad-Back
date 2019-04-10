# Student Testing System
[Russian README](README-RU.md)
## Description
System provides examination testing for students on the basis of the given test tasks.

Student choses a task he wants to complete and reads the terms.

He develops the program according to the task using one of the avaliable programming languages.

After sending the solution the System executes the code and inputs test datasets. For being solved correctly the program must output the correct results for all test inputs.

## How to run:

### WebApp 
Represents the System BackEnd storing all the neccessary data.

First, install .Net Core 2.1


1. Create _./WebApp/appsettings.Secret.json_ file

2. Fill _./WebApp/appsettings.Secret.json_ with the following: 

```json
{
    "JwtIssuerOptions": {
        "SecretKey":"Random string nearly 30 symbols"
    },
    "ConnectionStrings": {
        "PostgresDataBase": "The connection string to the Postgres database"
    }
}
```

You must use connection string to your real Postgres database in the field  ```ConnectionStrings:PostgresDataBase```, for example:
> _User ID=postgres;Password=password;Server=127.0.0.1;Port=5432;Database=TestDBdotnet;Integrated Security=true;_ 


3. After setting up the DB you must apply migration to the existing empty DB. For this you must run ```dotnet ef database update``` in WebApp folder. The command will create the required DB with all tables needed.


4. You can run the application using ```dotnet run INIT_ROLES=true```

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
    recaptchaClientToken: 'reCAPCHA public token'
};
```
> baseUrl - url of running WebApp, when running locally it will most likely be ```http://localhost:64800```
 
> recaptchaClientToken - **PUBLIC** token got when creating reCAPCTCHA v2

2. After running the site, you must execute ```npm start```. After this you can see the site in your browser on [localhost:4200](http://localhost:4200).

### Executor

Apllication connected to WebApp which is executing received solutions and checks them.

You must have **Docker** installed on the system you want to run Executor on. Actually **Docker** executes the received programs.


1. Create _./Executor/appsettings.Secret.json_

2. Fill it with the following:

```json
{
    "StartSettings": {
        "Address":"http://localhost:55471",
        "DockerEndPoint":"http://localhost:2375"
    },
    "UserInfo": {
        "UserName":"Username having Executor rights",
        "Password":"User password"
    }
}
```
> ```StartSettings:DockerEndPoint``` represents the url, pointing on **Docker** service, when running locally **Docker** is listening on port *2375*

3. After creating the file you can execute ```dotnet run``` in _./Executor_ folder.