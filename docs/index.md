# DataExplorer

[![Build Status](https://github.com/MikyM/DataExplorer/actions/workflows/release.yml/badge.svg)](https://github.com/MikyM/DataExplorer/actions)

Library featuring a few popular patterns in regard to working with data in .NET offering abstractions and implementations for EF Core and MongoDb.

To utilize all features using Autofac is required.

## Features

- Full fledged base entity definition with soft deletion and snowflake ID support
- Specification pattern encapsulating query LINQ logic avoiding bloated repositories
- Read-only and CRUD versions of repositories and data services
- Data services with mapping support (currently requires setting AutoMapper up)
- Fully abstracted and unit test ready including EF Core's DbContext
- Only asynchronous operations
- Supports decorators, adapters and interceptors via Autofac's methods
- MongoDb support via [MongoDb.Entities](https://mongodb-entities.com/)
- EF Core caching via a caching interceptor offered by [EFCoreSecondLevelCacheInterceptor](https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor)
- In-memory query evaluation through specifications

## Installation

To register the library services with the DI container use extension methods on `ContainerBuilder` or `IServiceCollection` provided by the library and register one or both offered sets of services:

```csharp
builder.AddDataExplorer(options => 
{
    options.AddEfCore(assembliesToScan);
    options.AddMongoDb();
});
```

## Description

Detailed descriptions are provided within EfCore and MongoDb directories (packages).