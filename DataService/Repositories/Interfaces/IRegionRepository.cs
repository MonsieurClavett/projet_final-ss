using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.DataService.Repositories.Interfaces
{
    public interface IRegionRepository
    {
        List<Region> GetAll();
        Task<List<Region>> GetAllAsync();

        Task<Region> AddAsync(Region region);
        Task<bool> DeleteAsync(Region region);
    }
}
