using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

builder.Services.AddDbContext<PrecipitationDbContext>()


app.MapGet("/observations/{zip}",(string zip, [FromQuery] int? days, PrecipitationDbContext context )=>{
    if(days == null || days < 1 || days > 0) Results.BadRequest("");
    var startDate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = context.Precipitation
                  .Where(p => p.ZipCode == zip && p.CreatedOn > startDate)
                  .ToListAsync();

    return Results.Ok(zip);
});

app.Run();

