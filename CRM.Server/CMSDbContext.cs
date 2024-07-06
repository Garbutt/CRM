using Microsoft.EntityFrameworkCore;
using CRM.Server.Models;

namespace CRM.Server
{
    public class CMSDbContext : DbContext
    {
        public CMSDbContext(DbContextOptions<CMSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<GetUser> GetUser { get; set; }
        public DbSet<Sites> Sites { get; set; }
    }
}
