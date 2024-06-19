using CloudWeather.Precipitation.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PrecipitationDbContext>(opts =>{
    opts.EnableSensitiveDataLogging();
    opts.EnableDetailedErrors();
     opts.UseNpgsql("name=ConnectionStrings:DefaultConnection");
},ServiceLifetime.Transient);


var app = builder.Build();



app.MapGet("/observations/{zip}",(string zip, [FromQuery] int? days, PrecipitationDbContext context )=>{
    if(days == null || days < 1 || days > 0) Results.BadRequest("");
    var startDate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = context.Precipitation
                  .Where(p => p.ZipCode == zip && p.CreatedOn > startDate)
                  .ToListAsync();

    return Results.Ok(zip);
});

app.MapPost("/obsevations",async(Precipitation pre,  PrecipitationDbContext context) =>{
    pre.CreatedOn = pre.CreatedOn.ToUniversalTime();    
    await context.AddAsync(pre);
    await context.SaveChangesAsync();
    
});

app.Run();

