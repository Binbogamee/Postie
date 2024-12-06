FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["LoggingService/LoggingService.csproj", "LoggingService/"]

RUN dotnet restore "LoggingService/LoggingService.csproj"

COPY ["Shared/", "Shared/"]
COPY ["LoggingService/", "LoggingService/"]

WORKDIR "/src/"
RUN dotnet publish "LoggingService/LoggingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoggingService.dll"]