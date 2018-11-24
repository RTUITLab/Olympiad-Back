FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Olympiad.sln ./
COPY WebApp/WebApp.csproj WebApp/
COPY Models/Models.csproj Models/
COPY Executor/Executor.csproj Executor/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/WebApp
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM node as node
WORKDIR /app
COPY ./ClientApp/package.json /app/
RUN npm install
COPY ./ClientApp /app/
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=node /app/dist/ ./wwwroot
RUN ls
RUN ls wwwroot
ENTRYPOINT ["dotnet", "WebApp.dll"]
