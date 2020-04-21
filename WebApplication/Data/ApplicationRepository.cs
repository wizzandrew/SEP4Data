using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication.Data.Entities;
using WebApplication.Models;

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
            //finding MetricsEntity with the biggest id -> last updated
            return await _context.Metrics.FindAsync(_context.Metrics.Max(max => max.MetricsID));
      
        }

        public async Task<MetricsEntity> getMetricsById(int id)
        {
            //finding MetricsEntity with the corresponding id using LINQ
            return await _context.Metrics.FindAsync(id);
        }

        //MetricsController---------------------------------------------------------------------------


        //RecommendedLevelsController-----------------------------------------------------------------

        public async Task<RecommendedLevelsEntity> GetRecommendedLevels()
        {
            //getting RecommendedLevelsEntity 
            return (await _context.RecommendedLevels.ToListAsync()).ElementAt(0);
        }

        //RecommendedLevelsController-----------------------------------------------------------------
    }
}
