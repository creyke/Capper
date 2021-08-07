using Capper.Samples.AspNet.Model;
using Capper.Samples.AspNet.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Pages
{
    public class VehicleModel : PageModel
    {
        private readonly ILogger<VehicleModel> _logger;
        private readonly IDistributedCache _cache;
        private readonly VehicleRepository _repository;

        public CacheResponse<IEnumerable<Vehicle>> CacheResponse { get; private set; }
        public string Variant { get; private set; }

        public VehicleModel(ILogger<VehicleModel> logger, IDistributedCache cache, VehicleRepository repository)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task OnGet(string id)
        {
            CacheResponse = await _cache.ReadThroughWithResponseAsync(id, async () =>
                await _repository.GetAsync(id));
            Variant = id;
        }
    }
}
