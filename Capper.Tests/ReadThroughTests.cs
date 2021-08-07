using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
            var response = await cache.ReadThroughAsync(key, () =>
                Task.FromResult((IEnumerable<string>)(new[] { key.ToString() })));

            Assert.Equal(key.ToString(), response.First());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CanReadDapper(int key)
        {
            var connection = new SqlConnection();

            var response = await cache.ReadThroughAsync(key, async () => 
                await connection.QueryAsync<string>("SELECT * FROM c"));

            Assert.Equal(key.ToString(), response.First());
        }
    }
}
