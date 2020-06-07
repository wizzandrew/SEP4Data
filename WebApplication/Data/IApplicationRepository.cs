using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public interface IApplicationRepository
    {
        public Task<MetricsEntity> getMetricsById(int metricsId);
        public Task<MetricsEntity> getLastUpdatedMetrics(int productID);
        public Task<List<MetricsEntity>> getMetricsForTimePeriod(DateTime start, DateTime end, int productID);
        public Task<UserEntity> GetUserEntity(string UserID);
        public Task<UserEntity> CreateAccount(UserEntity user);
        public Task<UserEntity> DeleteUser(UserEntity user);

    }
}
