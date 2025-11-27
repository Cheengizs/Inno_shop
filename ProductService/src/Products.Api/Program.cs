using Microsoft.EntityFrameworkCore;
using Products.Api.Configuration;
using Products.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Microsoft.Data.SqlClient.SqlException ex)
    {
        // logger logic
    }
    catch (Exception ex)
    {
        // other logger logic
        throw; 
    }
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();