using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Se puede usar el ServicioA o ServicioB u otro implementado en la interfaz en el futuro.
            //var autoresController = new AutoresController(new ApplicationDbContext(null), null, new ServicioA());
            
            //autoresController.Get();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureService(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Inyección de dependencias...
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            // Resolver dependencias
            // Se dice que cuando una clase requiera un IServicio entonces pasale un ServicioA
            // Con eso se instancian las dependencias de las dependencias de las clases
            services.AddTransient<IServicio, ServicioA>();

            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            services.AddResponseCaching();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            // O solo una clase (Clase como servicio)
            //services.AddTransient<ServicioA>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Login respuestas
            //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            
            // No exponemos la clase
            app.UseLoguearRespuestaHTTP();

            // Condicionar a ruta específica
            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsJsonAsync("Estoy interceptando la tubería.");
                });
            });



            if (env.IsDevelopment())
            {
                // Estos son middlewares (dicen use)
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Estos son middlewares
            app.UseHttpsRedirection();
            app.UseRouting();

            // El orden de los middlewares es muy importante.
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
