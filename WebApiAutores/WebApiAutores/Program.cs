using Microsoft.AspNetCore.Builder;
using WebApiAutores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Movido a Startup ConfigureService 

var startup = new Startup(builder.Configuration);
startup.ConfigureService(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.

// Movido a Startup

// Obtener instancia del ILogger
var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));
startup.Configure(app, app.Environment, servicioLogger);

app.Run();
