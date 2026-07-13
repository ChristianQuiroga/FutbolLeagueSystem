
using FutbolLeague.Application.DTOs;
using FutbolLeague.Application.Exceptions;
using FutbolLeague.Application.Services;
using FutbolLeague.Domain;
using FutbolLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Ctrol + M, O para colapsar todo el código y tener una vista general del controlador
namespace FutbolLeague.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //TEST
        [Authorize]
        [HttpGet("secure-test")]
        public IActionResult SecureTest()
        {
            return Ok("Entraste con token válido");
        }

        // GET: api/Categories
        /// <summary>
        /// Obtiene todas las categorías activas.
        /// </summary>
        /// <remarks>
        /// Devuelve únicamente las categorías disponibles para su uso.
        /// Las categorías desactivadas mediante borrado lógico no se incluyen.
        ///
        /// Este endpoint es público y no requiere autenticación.
        /// </remarks>
        /// <returns>Lista de categorías activas.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }


        // POST: api/Categories
        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <remarks>
        /// Registra una nueva categoría en el sistema.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="dto">Datos necesarios para crear la categoría.</param>
        /// <returns>La categoría creada.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);

            return Ok(category);
        }

        //Delete: api/Categories/{id}
        /// <summary>
        /// Desactiva una categoría.
        /// </summary>
        /// <remarks>
        /// Realiza un borrado lógico. La categoría permanece almacenada en la
        /// base de datos, pero su propiedad IsActive cambia a false y deja de
        /// aparecer en las consultas de categorías activas.
        ///
        /// Requiere autenticación mediante JWT y rol Admin.
        /// </remarks>
        /// <param name="id">Identificador de la categoría.</param>
        /// <returns>Confirmación de la desactivación.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Categoría desactivada correctamente"
            });
        }
    }
}