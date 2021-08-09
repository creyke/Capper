# Capper
Simple and powerful strongly typed read-through caching extensions for .NET's IDistributedCache

[![.NET Core](https://github.com/creyke/Capper/workflows/.NET%20Core/badge.svg)](https://github.com/creyke/Capper/actions?query=workflow%3A%22.NET+Core%22)
[![NuGet](https://img.shields.io/nuget/v/Capper.svg?style=flat)](https://www.nuget.org/packages/Capper)

# Overview
Capper provides extension methods for [IDistributedCache](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed) which encapsulate the business logic (and drastically simplify the implementation) of read-through caching.

It is partly inspired by the way [Dapper](https://github.com/DapperLib/Dapper) provides light, unopinionated strongly typed ORM extensions to **SqlConnection** (i.e. Capper is Dapper for caching).

Because it utilises **IDistributedCache**, Capper supports either in-memory (default) or [durable caches](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed#establish-distributed-caching-services).

# Status
The library is in general availability and is available as a [NuGet package](https://www.nuget.org/packages/Capper).

# Usage
Adding read through caching to your .NET application has never been easier.

## Installing
Install the [NuGet package](https://www.nuget.org/packages/Capper) with a package manager.

## Before Capper (without caching)
```csharp
private readonly AnimalRepository repository;

public AnimalController(AnimalRepository repository)
{
    this.repository = repository;
}

[HttpGet("{id}")]
public async Task<Animal> Get(string id)
{
    return await repository.GetAsync(id);
}
```

## After Capper
```csharp
private readonly AnimalRepository repository;
private readonly IDistributedCache cache;

public AnimalCacheController(AnimalRepository repository, IDistributedCache cache)
{
    this.repository = repository;
    this.cache = cache;
}

[HttpGet("{id}")]
public async Task<Animal> Get(string id)
{
    return await cache.ReadThroughAsync(id,
        async () => await repository.GetAsync(id));
}
```

## Adding durability (Redis in this case)
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "127.0.0.1:6379";
        options.InstanceName = "Animals";
    });

    services.AddRazorPages();
}
```

## Customising cache entry serialization / deserialziation
```csharp
public class JsonCacheSerializer : ICacheSerializer
{
    public T Deserialize<T>(byte[] serialized)
    {
        return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized));
    }

    public byte[] Serialize<T>(T deserialized)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deserialized));
    }
}

public async Task<Animal> Get(string id)
{
    return await cache.ReadThroughAsync(id,
        async () => await repository.GetAsync(id),
        new JsonCacheSerializer());
}
```

## Using Capper with Dapper
```csharp
[HttpGet("{id}")]
public async Task<Animal> Get(string id)
{
    return await cache.ReadThroughAsync(animalId, async () => { return
        await connection.QuerySingleAsync<Animal>($"SELECT TOP 1 FROM Animals WHERE Id = @AnimalId", new { AnimalId = animalId });
    });
}
```

## Using multiple caches in the same application
```csharp
public interface IAnimalCache : IDistributedCache {}
public class AnimalCache : MemoryDistributedCache, IAnimalCache
{
    public AnimalCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor) : base(optionsAccessor)
    {
    }
}

public interface IVehicleCache : IDistributedCache {}
public class VehicleCache : RedisCache, IVehicleCache
{
    public VehicleCache(IOptions<RedisCacheOptions> optionsAccessor) : base(optionsAccessor)
    {
    }
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IAnimalCache, AnimalCache>();
    services.AddSingleton<IVehicleCache, VehicleCache>();
}
```
