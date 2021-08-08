using Capper.Samples.AspNet.Model;
using Capper.Samples.AspNet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
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
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AnimalCacheController : ControllerBase
    {
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
    }
}
