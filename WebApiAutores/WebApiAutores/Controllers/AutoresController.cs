using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using Microsoft.Extensions.Configuration;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Ruta Base, ruta del controlador
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context; // Ahora es accesible desde cualquier parte de esta clase
        private readonly IConfiguration configuration;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public ServicioTransient ServicioTransient { get; }
        public ServicioScoped ServicioScoped { get; }
        public ServicioSingleton ServicioSingleton { get; }

        // IServicio es una dependencia de la clase
        // Se dice que IServicio se inyecta a travéz del constructor de la clase.
        public AutoresController(ApplicationDbContext context, IConfiguration configuration, IServicio servicio,
                    ServicioTransient servicioTransient, ServicioScoped servicioScoped, ServicioSingleton servicioSingleton,
                    ILogger<AutoresController> logger)
        {
            this.context = context;
            this.configuration = configuration;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("configuraciones")]
        public ActionResult<string> ObtenerConfiguracion()
        {
            return configuration["apellido"];
            //return configuration["ConnectionStrings:defaultConnection"];
        }


        [HttpGet("GUID")]
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObtenerTransient(),
                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.ObtenerScoped(),
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.ObtenerSingleton()
            });
        }


        [HttpGet]   // api/Autores
        [HttpGet("listado")] // api/Autores/listado
        [HttpGet("/listado")] // listado
        public async Task<List<Autor>> Get()
        {
            logger.LogInformation("Estamos obteniendo los autores.");
            logger.LogWarning("Este es un mensaje Warning");
            servicio.RealizarTarea();
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("primero")]  // api/autores/primero?nombre=Ali&apellido=Cruz
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }


        [HttpGet("primero2")]  // api/autores/primero
        public ActionResult<Autor> PrimerAutor2()
        {
            return new Autor() { Nombre = "Ali" }; // Desde memoria.
        }


        // Se vuelve una plantilla, ya que primero pide un entero diagonal un parametro string.
        // Se vuelve opcional si se le agrega el signo ?
        //[HttpGet("{id:int}/{param2?}")] // Variable de ruta opcional.
        [HttpGet("{id:int}/{param2=valorPorDefecto}")] // Variable de ruta con valor por defecto.
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }


        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));

            if (autor == null)
            {
                return NotFound();
            }

            return autor;
        }



        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        // Se concatena a la ruta base.
        [HttpPut("{id:int}")] // api/autores/algo
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL");

            }

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")] // api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }


    }
}
