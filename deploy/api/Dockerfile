FROM mcr.microsoft.com/dotnet/core/aspnet:8.0

WORKDIR /app

COPY api-build .

ENV ASPNETCORE_URLS=http://*:5501
CMD [ "dotnet", "WebApp.dll" ]