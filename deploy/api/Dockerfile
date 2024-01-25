FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY api-build .

ENV HTTP_PORTS=5501
CMD [ "dotnet", "WebApp.dll" ]