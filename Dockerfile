FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY MealplannerServer/*.csproj ./MealplannerServer/
COPY MealplannerUnittests/*.csproj ./MealplannerUnittests/
COPY MealplannerIntegrationtests/*.csproj ./MealplannerIntegrationtests/
COPY MealplannerCore/*.csproj ./MealplannerCore/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/MealplannerServer
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/MealplannerServer/out ./
ENTRYPOINT ["dotnet", "MealplannerServer.dll"]