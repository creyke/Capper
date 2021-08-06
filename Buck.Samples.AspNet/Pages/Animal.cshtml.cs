using Buck.Samples.AspNet.Caches;
using Buck.Samples.AspNet.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Buck.Samples.AspNet.Pages
{
    public class AnimalModel : PageModel
    {
        private readonly ILogger<AnimalModel> _logger;
        private readonly AnimalCache _cache;

        public CacheResponse<Animal[]> CacheResponse { get; private set; }
        public string Variant { get; private set; }

        public AnimalModel(ILogger<AnimalModel> logger, AnimalCache cache)
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
