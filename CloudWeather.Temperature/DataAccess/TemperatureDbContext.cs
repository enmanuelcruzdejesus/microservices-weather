using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temperature.DataAcces;



public class TemperatureDbContext : DbContext{
    
    public DbSet<Temperature> Temperature {get;set;}

    public TemperatureDbContext(){}
    public TemperatureDbContext(DbContextOptions options)   : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);
        SnakeCaseIdentityTableName(modelBuilder);

    }

      private static void SnakeCaseIdentityTableName(ModelBuilder modelBuilder){
        modelBuilder.Entity<Temperature>(w => w.ToTable("temperature"));
    }
}