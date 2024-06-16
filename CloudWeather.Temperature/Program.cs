using CloudWeather.Temperature.DataAcces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<TemperatureDbContext>(opts =>{
    opts.EnableSensitiveDataLogging();
    opts.EnableDetailedErrors();
     opts.UseNpgsql("name=ConnectionStrings:DefaultConnection");
},ServiceLifetime.Transient);


var app = builder.Build();


app.Run();

