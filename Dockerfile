FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY RawPotionServer/*.csproj ./RawPotionServer/
COPY RawPotionUnittests/*.csproj ./RawPotionUnittests/
COPY RawPotionIntegrationtests/*.csproj ./RawPotionIntegrationtests/
COPY RawPotionCore/*.csproj ./RawPotionCore/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/RawPotionServer
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/RawPotionServer/out ./
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "RawPotionServer.dll"]