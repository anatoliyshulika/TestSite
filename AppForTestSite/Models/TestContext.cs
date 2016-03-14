using System.Data.Entity;

namespace AppForTestSite
{
    public class TestContext : DbContext
    {
        public DbSet<Website> Websites { get; set; }
    }
}