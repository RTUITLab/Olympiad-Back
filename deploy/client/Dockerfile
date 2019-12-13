FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY Olympiad/olympiad-back-client .
COPY Olympiad/olympiad-front-client ./wwwroot/

CMD [ "dotnet", "WebApp.dll" ]