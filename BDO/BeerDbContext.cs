using Microsoft.EntityFrameworkCore;

namespace BDO_Project.BDO
{
    /// <summary>
    /// Uses a in memory db to simplify this project.
    /// </summary>
    public class BeerDbContext : DbContext
    {
        public DbSet<ItemBeer> Beers { get; set; }

        public BeerDbContext(DbContextOptions<BeerDbContext>options) : base(options)
        {
        }

    }
}
