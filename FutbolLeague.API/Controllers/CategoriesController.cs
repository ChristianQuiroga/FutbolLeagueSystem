
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
        // Obtiene todas las categorías
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }


        // POST: api/Categories
        // Crea una nueva categoría
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);

            return Ok(category);
        }

        //Delete: api/Categories/{id}
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