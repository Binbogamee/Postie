FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ARG CONNECTION_STRING
ENV CONNECTION_STRING=$CONNECTION_STRING

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY ["Postie.DAL/Postie.DAL.csproj", "Postie.DAL/"]
COPY ["Postie/Postie.csproj", "Postie/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Postie/Postie.csproj"

COPY ["Postie.DAL/", "Postie.DAL/"]
COPY ["Postie/", "Postie/"]
COPY ["Shared/", "Shared/"]

RUN dotnet ef migrations add init -s /src/Postie -p /src/Postie.DAL