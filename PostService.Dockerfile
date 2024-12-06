FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Postie.DAL/Postie.DAL.csproj", "Postie.DAL/"]
COPY ["Postie/Postie.csproj", "Postie/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["PostService/PostService.csproj", "PostService/"]

RUN dotnet restore "PostService/PostService.csproj"

COPY ["Postie.DAL/", "Postie.DAL/"]
COPY ["Postie/", "Postie/"]
COPY ["Shared/", "Shared/"]
COPY ["PostService/", "PostService/"]

WORKDIR "/src/"
RUN dotnet publish "PostService/PostService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostService.dll"]