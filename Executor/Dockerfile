FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Olympiad.sln ./
COPY Models/Models.csproj Models/
COPY Executor/Executor.csproj Executor/
COPY WebApp/WebApp.csproj WebApp/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Executor
RUN dotnet build -c Release -o /app
RUN dotnet publish -c Release -o /app

FROM microsoft/dotnet:2.1-runtime AS final
COPY --from=build /app /app
WORKDIR /app
CMD dotnet "Executor.dll"