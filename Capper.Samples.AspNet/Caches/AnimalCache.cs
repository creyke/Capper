using Capper.Samples.AspNet.Model;
using Capper.Samples.AspNet.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Caches
{
    public class AnimalCache : Cache<string, Animal[]>
    {
        private readonly AnimalRepository repository;

        public AnimalCache(AnimalRepository repository)
        {
            this.repository = repository;
        }

        protected async override Task<Animal[]> HydrateAsync(string key)
        {
            return (await repository.GetAsync(key)).ToArray();
        }
    }
}
