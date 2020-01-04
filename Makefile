all: unittest integrationtest build

build:
	dotnet build

build-prod:
	dotnet publish -c Release -o out

test: unittest integrationtest 

unittest:
	dotnet test MealplannerUnittests

integrationtest: 
	dotnet test MealplannerIntegrationtests

run:
	dotnet run --project MealplannerServer

deps:
	dotnet restore

docker-build:
	docker build --tag kjuulh/mealplanner-server:${MEALPLANNER_VERSION} .