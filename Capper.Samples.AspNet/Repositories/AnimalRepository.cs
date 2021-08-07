using Capper.Samples.AspNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Repositories
{
    public class AnimalRepository
    {
        private readonly Random random;

        public AnimalRepository()
        {
            random = new Random();
        }

        public async Task<IEnumerable<Animal>> GetAsync(string key)
        {
            await Task.Delay(random.Next(100, 300));

            return Enumerable.Range(0, random.Next(5, 10)).Select(x => new Animal
            {
                Id = Guid.NewGuid(),
                Variant = key
            });
        }
    }
}
