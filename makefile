# the version of of https://github.com/nitrictech/nitric to use in base client generation.
NITRIC_VERSION := 0.18.0

generate: clean download build

build: 
	dotnet build src/Nitric.Client/Nitric.Client.csproj

clean:
	rm -rf src/Nitric.Client/bin
	rm -rf src/Nitric.Client/obj
	rm -rf src/Nitric.Client/Proto

download:
	curl -L https://github.com/nitrictech/nitric/releases/download/v${NITRIC_VERSION}/contracts.tgz -o contracts.tgz
	tar xvzf contracts.tgz
	rm contracts.tgz