using Final.DataService.Repositories.Interfaces;
using Final.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.DataService.Repositories.Database
{
    public class RegionDatabaseRepository : BaseDatabaseRepository, IRegionRepository
    {
        public RegionDatabaseRepository(MeteoDbContext context) : base(context) { }

        public List<Region> GetAll()
        {
            return _context.Regions.OrderBy(r => r.Nom).ToList();
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await _context.Regions.OrderBy(r => r.Nom).ToListAsync();
        }

        public async Task<Region> AddAsync(Region region)
        {
            _context.Regions.Add(region);
            await _context.SaveChangesAsync();
            return region;
        }

        public async Task<bool> DeleteAsync(Region region)
        {
            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
