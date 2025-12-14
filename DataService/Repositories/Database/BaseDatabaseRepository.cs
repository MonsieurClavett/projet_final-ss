using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.DataService.Repositories.Database
{
    public abstract class BaseDatabaseRepository
    {
        protected readonly MeteoDbContext _context;

        protected BaseDatabaseRepository(MeteoDbContext context)
        {
            _context = context;
        }
    }
}
