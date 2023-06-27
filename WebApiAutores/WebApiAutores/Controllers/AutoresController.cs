using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Authorization;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Ruta Base, ruta del controlador
    
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context; // Ahora es accesible desde cualquier parte de esta clase

        // IServicio es una dependencia de la clase
        // Se dice que IServicio se inyecta a travéz del constructor de la clase.
        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }


        [HttpGet]   // api/Autores
        //[Authorize]
        public async Task<List<Autor>> Get()
        {
            return await context.Autores.ToListAsync();
        }

        // Se vuelve una plantilla, ya que primero pide un entero diagonal un parametro string.
        // Se vuelve opcional si se le agrega el signo ?
        //[HttpGet("{id:int}/{param2?}")] // Variable de ruta opcional.
        [HttpGet("{id:int}")] // Variable de ruta con valor por defecto.
        public async Task<ActionResult<Autor>> Get(int id)
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
