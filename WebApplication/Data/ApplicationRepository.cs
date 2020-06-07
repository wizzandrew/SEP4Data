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
        public async Task<MetricsEntity> getLastUpdatedMetrics(int productID)
        {
            //finding MetricsEntity with the latest updated date 
            if (_context.Metrics.Any(m => m.ProductID == productID && m.LastUpdated.HasValue))
            {
                DateTime latestDate = _context.Metrics.Where(m => m.ProductID == productID && m.LastUpdated.HasValue).Max(max => max.LastUpdated).Value;

                //apparently there can be multiple metrics with same date (exact same down til seconds)
                List<MetricsEntity> metrics =  await _context.Metrics.Where(m => m.LastUpdated.Value.CompareTo(latestDate) == 0
                                                   && m.ProductID == productID).ToListAsync();
                return metrics[metrics.Count-1];
            }
            return null;
        }

        //MetricsController---------------------------------------------------------------------------


        //StatisticsController----------------------------------------------------------------------------

        public async Task<List<MetricsEntity>> getMetricsForTimePeriod(DateTime start, DateTime end, int productID)
        {
            if (_context.Metrics.Any(m => m.ProductID == productID &&  m.LastUpdated.HasValue))
            {
                return await _context.Metrics.Where(m => m.LastUpdated.Value >= start && m.LastUpdated.Value <= end
                                                    && m.ProductID == productID).ToListAsync();
            }
            return null;


        }

        //StatisticsController-----------------------------------------------------------------------------


        //UserController----------------------------------------------------------------------------


        public async Task<UserEntity> GetUserEntity(string UserID)
        {
            if (_context.Users.Any(u => u.UserID.Length > 0))
            {
                return await _context.Users.FindAsync(UserID);
            }
            return null;
        }

        public async Task<UserEntity> CreateAccount(UserEntity user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            //return user added to Users db
            return await GetUserEntity(user.UserID);
        }

        public async Task<UserEntity> DeleteUser(UserEntity user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();

            //return user from db if FOUND (not expected)
            return await GetUserEntity(user.UserID);
        }

        //UserController----------------------------------------------------------------------------

    }
}
