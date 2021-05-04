# Nitric .NET SDK
The .NET SDK supports the use of the cloud-portable [Nitric](https://nitric.io) framework with .NET Standard 2.0.
> The SDK is in early stage development and APIs and interfaces are still subject to breaking changes

## Prerequisites
- .NET Standard 2.0+

## Getting Started


### Using the [Nitric CLI](https://github.com/nitric-tech/cli)
> The SDK is included in all C# related SDK projects by default

```bash
nitric make:function <csharp-example> example
```

Some available C# templates are:

* faas/dotnet2

### Adding to an existing project
**C#**
```xml
<ItemGroup>
    <PackageReference Include="Nitric.Sdk" Version="1.0.1" />
</ItemGroup>
```

## Usage
Code examples are available [here](https://nitrictech.github.io/dotnet-sdk/)