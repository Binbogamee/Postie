FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Postie.DAL/Postie.DAL.csproj", "Postie.DAL/"]
COPY ["Postie/Postie.csproj", "Postie/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["AccountService/AccountService.csproj", "AccountService/"]

RUN dotnet restore "AccountService/AccountService.csproj"

COPY ["Postie.DAL/", "Postie.DAL/"]
COPY ["Postie/", "Postie/"]
COPY ["Shared/", "Shared/"]
COPY ["AccountService/", "AccountService/"]

WORKDIR "/src/"
RUN dotnet publish "AccountService/AccountService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AccountService.dll"]