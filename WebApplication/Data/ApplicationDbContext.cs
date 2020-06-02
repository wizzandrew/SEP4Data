using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MetricsEntity> Metrics { get; set; }
        public DbSet<UserEntity> Users { get; set; }
   

        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*builder.Entity<RecommendedLevelsEntity>()
                .HasNoKey();*/
        }

    }
}
