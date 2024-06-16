using CloudWeather.Temperature.DataAcces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<TemperatureDbContext>(opts =>{
    opts.EnableSensitiveDataLogging();
    opts.EnableDetailedErrors();
     opts.UseNpgsql("name=ConnectionStrings:DefaultConnection");
},ServiceLifetime.Transient);


var app = builder.Build();



app.MapGet("/observation/{zip}",(string zip, [FromQuery] int? days, TemperatureDbContext context )=>{
    if(days == null || days < 1 || days > 0) Results.BadRequest("");
    var startDate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = context.Temperature
                  .Where(p => p.ZipCode == zip && p.CreatedOn > startDate)
                  .ToListAsync();

    return Results.Ok(zip);
});

app.MapPost("/obsevations",async(Temperature temp,  TemperatureDbContext context) =>{
    temp.CreatedOn = temp.CreatedOn.ToUniversalTime();    
    await context.AddAsync(temp);
    await context.SaveChangesAsync();
    
});

app.Run();

