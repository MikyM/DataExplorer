[![NuGet](https://img.shields.io/nuget/v/DataExplorer)](https://www.nuget.org/packages/DataExplorer)[![NuGet](https://img.shields.io/nuget/dt/DataExplorer
)](https://www.nuget.org/packages/DataExplorer)
[![Build Status](https://github.com/MikyM/DataExplorer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MikyM/DataExplorer/actions)
![GitHub License](https://img.shields.io/github/license/MikyM/DataExplorer)
[![Static Badge](https://img.shields.io/badge/Documentation-DataExplorer-Green)](https://mikym.github.io/DataExplorer)

# DataExplorer

Library featuring an opinionated, reusable data access layer offering abstractions and implementations for SQL storages (EF Core) and MongoDb (MongoDb.Entities).

To utilize all features using Autofac is required.

## Features

- Full fledged base entity definition with soft deletion and snowflake ID support
- Specification pattern encapsulating query LINQ logic avoiding bloated repositories
- Read-only and CRUD versions of repositories and data services
- Data services with mapping support (currently requires setting AutoMapper up)
- Fully abstracted and unit test ready including EF Core's DbContext
- Only asynchronous operations
- Supports decorators, adapters and interceptors via Autofac's methods
- EF Core caching via a caching interceptor offered by [EFCoreSecondLevelCacheInterceptor](https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor)
- In-memory query evaluation through specifications

## Installation

To register the library services with the DI container use extension methods on `ContainerBuilder` or `IServiceCollection` provided by the library and register one or both offered sets of services:

```csharp
builder.AddDataExplorer(options => 
{
    options.AddEfCore(assembliesToScan);
});
```
## Download

- `DataExplorer` - [NuGet](https://www.nuget.org/packages/DataExplorer)
- `DataExplorer.EfCore` - [NuGet](https://www.nuget.org/packages/DataExplorer.EfCore)

## Documentation

Documentation available at https://mikym.github.io/DataExplorer/.

## Examples

Examples and PoCs are available within the [examples](https://github.com/MikyM/DataExplorer/tree/develop/examples) subdirectory.
