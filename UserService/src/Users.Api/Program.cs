using Microsoft.EntityFrameworkCore;
using Users.Api.Configuration;
using Users.Infrastructure.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
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

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();
