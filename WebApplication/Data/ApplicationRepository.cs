using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication.Data.Entities;


namespace WebApplication.Data
{
    public class ApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //MetricsController---------------------------------------------------------------------------
        public async Task<MetricsEntity> getLastUpdatedMetrics()
        {
            //finding MetricsEntity with the latest updated date 
            if (_context.Metrics.Any(m => m.LastUpdated.HasValue))
            {
                DateTime latestDate = _context.Metrics.Where(m => m.LastUpdated.HasValue).Max(max => max.LastUpdated).Value;
                return await _context.Metrics.Where(m => m.LastUpdated.Value == latestDate).FirstAsync();
            }
            return null;
        }

        //MetricsController---------------------------------------------------------------------------


        //StatisticsController----------------------------------------------------------------------------

        public async Task<List<MetricsEntity>> getMetricsForTimePeriod(DateTime start, DateTime end)
        {
            if (_context.Metrics.Any(m => m.LastUpdated.HasValue))
            {
                return await _context.Metrics.Where(m => m.LastUpdated.Value >= start &&
                                                      m.LastUpdated.Value <= end).ToListAsync();
            }
            return null;
        }

        //StatisticsController-----------------------------------------------------------------------------


        //UserController----------------------------------------------------------------------------

        public async Task<UserEntity> GetUserEntity(string email)
        {
            if (_context.Users.Any(u => u.Email.Length > 0))
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            }
            return null;
        }

        //UserController----------------------------------------------------------------------------

    }
}
