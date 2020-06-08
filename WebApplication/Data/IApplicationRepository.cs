using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public interface IApplicationRepository
    {
        public Task<MetricsEntity> GetMetricsById(int metricsId);
        public Task<MetricsEntity> GetLastUpdatedMetrics(int productID);
        public Task<List<MetricsEntity>> GetMetricsForTimePeriod(DateTime start, DateTime end, int productID);
        public Task<UserEntity> GetUserEntity(string userID);
        public Task<UserEntity> CreateAccount(UserEntity user);
        public Task<UserEntity> DeleteAccount(UserEntity user);

    }
}
