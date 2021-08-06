using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Buck.Tests
{
    public class CacheTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CanRead(int key)
        {
            var subject = new PingPongCache();

            var response = await subject.ReadAsync(key);

            Assert.Equal(key.ToString(), response.Value);
        }

        [Theory]
        [InlineData(0)]
        public async Task CanHitCacheOnSecondAttempt(int key)
        {
            var subject = new PingPongCache();

            var first = await subject.ReadAsync(key);

            Assert.Equal(CacheResponseType.Miss, first.ResponseType);
            Assert.Equal(key.ToString(), first.Value);

            var second = await subject.ReadAsync(key);

            Assert.Equal(CacheResponseType.Hit, second.ResponseType);
            Assert.Equal(key.ToString(), second.Value);
        }

        [Theory]
        [InlineData(0)]
        public async Task DoesntPersistOnException(int key)
        {
            var subject = new FailingFirstTimeCache();

            await Assert.ThrowsAsync<IndexOutOfRangeException>(() => subject.ReadAsync(key));

            var response = await subject.ReadAsync(key);

            Assert.Equal(CacheResponseType.Miss, response.ResponseType);
            Assert.Equal(key.ToString(), response.Value);
        }

        [Fact]
        public async Task CanMeasureMissAndHits()
        {
            var subject = new PingPongCache();
        }

        class PingPongCache : Cache<int, string>
        {
            public PingPongCache(IDistributedCache cache, ICacheMetricService metrics) : base(cache, metrics)
            {
            }

            public PingPongCache(IDistributedCache cache) : base(cache)
            {
            }

            public PingPongCache()
            {
            }

            protected override Task<string> HydrateAsync(int key)
            {
                return Task.FromResult(key.ToString());
            }
        }

        class FailingFirstTimeCache : Cache<int, string>
        {
            private int attempt;

            public FailingFirstTimeCache(IDistributedCache cache) : base(cache)
            {
            }

            public FailingFirstTimeCache()
            {
            }

            protected override Task<string> HydrateAsync(int key)
            {
                attempt++;

                if (attempt < 2)
                {
                    throw new IndexOutOfRangeException();
                }

                return Task.FromResult(key.ToString());
            }
        }
    }
}
