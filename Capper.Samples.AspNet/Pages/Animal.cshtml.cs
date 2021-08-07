using Capper.Samples.AspNet.Model;
using Capper.Samples.AspNet.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Pages
{
    public class AnimalModel : PageModel
    {
        private readonly ILogger<AnimalModel> _logger;
        private readonly IDistributedCache _cache;
        private readonly AnimalRepository _repository;

        public CacheResponse<IEnumerable<Animal>> CacheResponse { get; private set; }
        public string Variant { get; private set; }

        public AnimalModel(ILogger<AnimalModel> logger, IDistributedCache cache, AnimalRepository repository)
        {
            _logger = logger;
            _cache = cache;
            _repository = repository;
        }

        public async Task OnGet(string id)
        {
            CacheResponse = await _cache.ReadThroughWithResponseAsync(id, async () =>
                await _repository.GetAsync(id));
            Variant = id;
        }
    }
}
