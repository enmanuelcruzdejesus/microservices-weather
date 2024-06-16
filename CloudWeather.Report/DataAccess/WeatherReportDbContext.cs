using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Report.DataAccess;

public class WeatherReportDbContext : DbContext{
    public WeatherReportDbContext(DbContextOptions options) : base(options) { }

    public DbSet<WeatherReport> WeeklyWeatherReports {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SnakeCaseIdentityTableName(modelBuilder);
    }

    private static void SnakeCaseIdentityTableName(ModelBuilder modelBuilder){
        modelBuilder.Entity<WeatherReport>(w => w.ToTable("weather_report"));
    }

}