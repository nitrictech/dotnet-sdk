# the version of of https://github.com/nitrictech/nitric to use in base client generation.
NITRIC_VERSION := 0.18.0

generate: clean download build
publish: clean download build pack

build: 
	dotnet build src/Nitric.Sdk/Nitric.Sdk.csproj

clean:
	rm -rf src/Nitric.Sdk/bin
	rm -rf src/Nitric.Sdk/obj
	rm -rf src/Nitric.Sdk/Proto

download:
	curl -L https://github.com/nitrictech/nitric/releases/download/v${NITRIC_VERSION}/contracts.tgz -o contracts.tgz
	tar xvzf contracts.tgz
	rm contracts.tgz

pack:
	mkdir __out
	dotnet pack -c Release -o __out
	dotnet nuget push "./__out/*.nupkg" --skip-duplicate --no-symbols true --api-key ${{secrets.NUGET_TOKEN}}
	rm -rf __out