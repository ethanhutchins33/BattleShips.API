FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

COPY ["global.json", "."]
COPY ["BattleShips.API.sln", "."]
COPY ["BattleShips.API/BattleShips.API.csproj", "BattleShips.API/"]
COPY ["BattleShips.API.Data/BattleShips.API.Data.csproj", "BattleShips.API.Data/"]
COPY ["BattleShips.API.Library/BattleShips.API.Library.csproj", "BattleShips.API.Library/"]
RUN dotnet restore "BattleShips.API/BattleShips.API.csproj"

COPY . .
RUN dotnet publish "BattleShips.API/BattleShips.API.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BattleShips.API.dll"]
