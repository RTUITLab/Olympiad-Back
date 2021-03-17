FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY ./results-viewer-build /app

ENV ASPNETCORE_URLS=http://*:5504
EXPOSE 5504

CMD dotnet /app/ResultsViewer.dll