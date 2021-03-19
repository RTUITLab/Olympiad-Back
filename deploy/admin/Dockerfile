FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app

COPY admin-build .
ENV ASPNETCORE_URLS=http://*:5503
CMD [ "dotnet", "Admin.dll" ]