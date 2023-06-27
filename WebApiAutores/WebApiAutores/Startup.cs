using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;

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
            services.AddControllers(opciones =>
                {
                    opciones.Filters.Add(typeof(FiltroDeExcepcion));
                })
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            // Inyección de dependencias...
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));


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
