FROM registry.truesight.asia/aspnet:3.1-buster-slim AS base
WORKDIR /app
RUN apt-get update && apt-get install -y net-tools curl iputils-ping telnet nano vim libc6-dev libgdiplus

FROM registry.truesight.asia/dotnet-sdk:3.1-buster AS build
WORKDIR /src
COPY ["LocatingApp.csproj", "./"]
RUN dotnet restore "LocatingApp.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "LocatingApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LocatingApp.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

COPY ["docker-entrypoint.sh", "."]

RUN chmod a+x docker-entrypoint.sh

CMD ["./docker-entrypoint.sh"]

EXPOSE 80
