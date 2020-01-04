all: unittest integrationtest build

build:
	dotnet build

build-prod:
	dotnet publish -c Release -o out

test: unittest integrationtest 

unittest:
	dotnet test RawPotionUnittests

integrationtest: 
	dotnet test RawPotionIntegrationtests

run:
	dotnet run --project RawPotionServer

deps:
	dotnet restore

docker-build:
	docker build --tag kjuulh/RawPotion-server:${RawPotion_VERSION} .