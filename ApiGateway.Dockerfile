FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["AuthHelper/AuthHelper.csproj", "AuthHelper/"]
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]

RUN dotnet restore "ApiGateway/ApiGateway.csproj"

COPY ["Shared/", "Shared/"]
COPY ["AuthHelper/", "AuthHelper/"]
COPY ["ApiGateway/", "ApiGateway/"]

WORKDIR "/src/"
RUN dotnet publish "ApiGateway/ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]