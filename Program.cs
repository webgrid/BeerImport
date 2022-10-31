using BDO_Project.BDO;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BeerDbContext> (o => o.UseInMemoryDatabase("BeerDatabase"));
var app = builder.Build();
var worker = new BeerManager(app.Services.CreateScope().ServiceProvider.GetService<BeerDbContext>());
//A very simple app
app.MapGet("/", async (context) => 
{
    context.Response.Headers.Add("Content-Type", "text/html");
    await context.Response.WriteAsync(worker.BeersToHtml());
});
app.Run();


