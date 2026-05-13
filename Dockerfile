FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore OttoBot.fsproj
RUN dotnet publish OttoBot.fsproj -c release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0

COPY --from=build /app /app
RUN groupadd -r otto && useradd --no-log-init -r -g otto otto
USER otto

ENTRYPOINT ["/app/OttoBot"]