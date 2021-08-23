using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoWeb2.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoWeb2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")] 
    public class CategorysController: ControllerBase
    {
        private readonly DatabaseContext _context;

        public CategorysController(DatabaseContext context){
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(long id){
            var category = await _context.Categorys.FindAsync(id);
            if (category == null) {
                return NotFound();
            }
            return category;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategorys(){
            return await _context.Categorys.ToArrayAsync();
        }
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category){
            _context.Categorys.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new{id = category.id}, category);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> UpdateCategory(long id, Category category){
            if (id != category.id) {
                return BadRequest();
            }
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new { id = category.id }, category);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(long id){
            var category = await _context.Categorys.FindAsync(id);
            if (category == null) {
                return NotFound();
            }

            _context.Categorys.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}