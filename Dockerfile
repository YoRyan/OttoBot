FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore OttoBot/OttoBot.fsproj
RUN dotnet publish OttoBot/OttoBot.fsproj -c release -o /app --no-self-contained --no-restore

FROM mcr.microsoft.com/dotnet/runtime:6.0

RUN groupadd -r otto && useradd --no-log-init -r -g otto otto
USER otto
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./OttoBot"]