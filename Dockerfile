FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-self-contained --no-restore

FROM mcr.microsoft.com/dotnet/runtime:5.0

RUN groupadd -r otto && useradd --no-log-init -r -g otto otto
USER otto
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./OttoBot"]