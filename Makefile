.PHONY: build test

build:
	./scripts/dotnet-safe.sh build CBBSimulator.sln

test:
	./scripts/dotnet-safe.sh test CBBSimulator.sln
