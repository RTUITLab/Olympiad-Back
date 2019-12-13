FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app

COPY Olympiad/olympiad-client-back-build .
COPY Olympiad/olympiad-client-front-build ./wwwroot/

CMD [ "dotnet", "WebApp.dll" ]