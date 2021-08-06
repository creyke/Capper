using Buck.Samples.AspNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buck.Samples.AspNet.Repositories
{
    public class VehicleRepository
    {
        private readonly Random random;

        public VehicleRepository()
        {
            random = new Random();
        }

        public async Task<IEnumerable<Vehicle>> GetAsync(string key)
        {
            await Task.Delay(random.Next(100, 300));

            return Enumerable.Range(0, random.Next(5, 10)).Select(x => new Vehicle
            {
                Id = Guid.NewGuid(),
                Variant = key
            });
        }
    }
}
