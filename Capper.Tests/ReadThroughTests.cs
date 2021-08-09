using Capper.Serialization;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Capper.Tests
{
    public class ReadThroughTests
    {
        private readonly IDistributedCache cache;

        public ReadThroughTests()
        {
            cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CanRead(int key)
        {
            var response = await cache.ReadThroughAsync(key,
                () => Task.FromResult((IEnumerable<string>)(new[] { key.ToString() })));

            Assert.Equal(key.ToString(), response.First());
        }

        [Theory]
        [InlineData(0)]
        public async Task CanHitCacheOnSecondAttempt(int key)
        {
            var first = await cache.ReadThroughWithResponseAsync(key,
                () => Task.FromResult(key.ToString()));

            Assert.Equal(CacheResponseType.Miss, first.ResponseType);
            Assert.Equal(key.ToString(), first.Value);

            var second = await cache.ReadThroughWithResponseAsync(key,
                () => Task.FromResult(key.ToString()));

            Assert.Equal(CacheResponseType.Hit, second.ResponseType);
            Assert.Equal(key.ToString(), second.Value);
        }

        [Theory]
        [InlineData(0)]
        public async Task DoesntPersistOnException(int key)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => cache.ReadThroughWithResponseAsync(key,
                () => Task.FromResult(key.ToString()[int.MaxValue..])));

            var second = await cache.ReadThroughWithResponseAsync(key,
                () => Task.FromResult(key.ToString()));

            Assert.Equal(CacheResponseType.Miss, second.ResponseType);
            Assert.Equal(key.ToString(), second.Value);
        }

        [Theory]
        [InlineData(0)]
        public async Task CanUseCustomSerializer(int key)
        {
            var serializer = new PaddedJsonCacheSerializer();

            var first = await cache.ReadThroughWithResponseAsync(key,
                () => Task.FromResult(key.ToString()),
                serializer);

            Assert.Equal(CacheResponseType.Miss, first.ResponseType);
            Assert.Equal(key.ToString(), first.Value);

            var second = await cache.ReadThroughWithResponseAsync(key,
                () => Task.FromResult(key.ToString()),
                serializer);

            Assert.Equal(CacheResponseType.Hit, second.ResponseType);
            Assert.Equal(key.ToString(), second.Value);
        }

        [Theory(Skip = "Example integration test.")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CanReadDapper(int key)
        {
            var connection = new SqlConnection();

            var response = await cache.ReadThroughAsync(key,
                async () => await connection.QueryAsync<Car>($"SELECT * FROM Cars WHERE NumDoors = {key}"));
        }

        class Car
        {
        }

        class PaddedJsonCacheSerializer : ICacheSerializer
        {
            private readonly ICacheSerializer jsonCacheSerializer;

            public PaddedJsonCacheSerializer()
            {
                jsonCacheSerializer = new JsonCacheSerializer();
            }

            public T Deserialize<T>(byte[] serialized)
            {
                return jsonCacheSerializer.Deserialize<T>(serialized[0..^1]);
            }

            public byte[] Serialize<T>(T deserialized)
            {
                var array = jsonCacheSerializer.Serialize<T>(deserialized);
                Array.Resize(ref array, array.Length + 1);
                return array;
            }
        }
    }
}
