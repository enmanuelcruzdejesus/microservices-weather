namespace CloudWeather.Precipitation.DataAccess;
using Microsoft.EntityFrameworkCore;


public class PrecipitationDbContext: DbContext{
    public PrecipitationDbContext(DbContextOptions options ): base( options )   {}
    
    public DbSet<Precipitation> Precipitation { get; set;}

     protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);
        SnakeCaseIdentityTableName(builder);
    }

    public static void SnakeCaseIdentityTableName(ModelBuilder builder){
        builder.Entity<Precipitation>(p=> { p.ToTable("precipitation"); });
    }
}
