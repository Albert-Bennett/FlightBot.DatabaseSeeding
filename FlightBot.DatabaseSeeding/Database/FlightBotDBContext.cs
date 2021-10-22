using Microsoft.EntityFrameworkCore;

namespace FlightBot.DatabaseSeeding.Database.Entities
{
    public class FlightBotDBContext : DbContext
    {
        public FlightBotDBContext() { }
        public FlightBotDBContext(DbContextOptions<FlightBotDBContext> options) : base(options) { }

        public DbSet<IATACodeEntity> IATACode { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IATACodeEntity>().ToTable("IATACodes", "dbo");
        }
    }
}
