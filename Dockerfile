FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-self-contained --no-restore

FROM mcr.microsoft.com/dotnet/runtime:5.0

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./OttoBot"]