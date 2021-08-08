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
        private readonly AnimalRepository _repository;
        private readonly IDistributedCache _cache;
        
        public CacheResponse<string, IEnumerable<Animal>> CacheResponse { get; private set; }

        public AnimalModel(ILogger<AnimalModel> logger, AnimalRepository repository, IDistributedCache cache)
        {
            _logger = logger;
            _repository = repository;
            _cache = cache;
        }

        public async Task OnGet(string id)
        {
            CacheResponse = await _cache.ReadThroughWithResponseAsync(id,
                async () => await _repository.ListAsync(id));
        }
    }
}
