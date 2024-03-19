# the version of of https://github.com/nitrictech/nitric to use in base client generation.
NITRIC_VERSION := 1.1.1

generate: clean download build

test:
	@dotnet test tests/Nitric.Sdk.Test

build: 
	@dotnet build src/Nitric.Sdk/Nitric.Sdk.csproj
	@mv src/Nitric.Sdk/Proto/nitric/proto/*  src/Nitric.Sdk/Proto/
	@rm -rf nitric
	@rm -rf src/Nitric.Sdk/Proto/nitric

clean:
	@rm -rf src/Nitric.Sdk/bin
	@rm -rf src/Nitric.Sdk/obj
	@rm -rf src/Nitric.Sdk/Proto

download:
	@curl -L https://github.com/nitrictech/nitric/releases/download/v${NITRIC_VERSION}/proto.tgz -o nitric.tgz
	@tar xvzf nitric.tgz
	@rm nitric.tgz

pack: clean download
	@dotnet build src/Nitric.Sdk/Nitric.Sdk.csproj -c Release
	@mkdir -p __out
	@dotnet pack -c Release -o __out

coverage:
	@echo "Using dotcover tool... Install using: 'dotnet tool install --global JetBrains.dotCover.GlobalTool'"
	@rm -f coverage.xml dotCover.Output.html
	@dotnet dotcover test --dcReportType=HTML --dcFilters="+:Nitric.Sdk;-:type=Nitric.Proto.*"
	@open dotCover.Output.html