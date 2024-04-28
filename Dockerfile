FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY *.csproj .
RUN dotnet restore

COPY Astra.sln Astra.sln
COPY src src
RUN dotnet publish --no-restore -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./Astra"]
