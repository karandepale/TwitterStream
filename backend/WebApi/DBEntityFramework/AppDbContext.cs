using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebApi.DBEntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DataBaseEntity> PersonalProjectTable { get; set; }
    }
}
