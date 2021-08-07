using Capper.Samples.AspNet.Caches;
using Capper.Samples.AspNet.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Pages
{
    public class VehicleModel : PageModel
    {
        private readonly ILogger<VehicleModel> _logger;
        private readonly VehicleCache _cache;

        public CacheResponse<Vehicle[]> CacheResponse { get; private set; }
        public string Variant { get; private set; }

        public VehicleModel(ILogger<VehicleModel> logger, VehicleCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task OnGet(string id)
        {
            CacheResponse = await _cache.ReadAsync(id);
            Variant = id;
        }
    }
}
