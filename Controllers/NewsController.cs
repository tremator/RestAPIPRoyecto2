using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoWeb2.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoWeb2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")] 
    public class NewsController:ControllerBase
    {
        private readonly DatabaseContext _context;

        public NewsController(DatabaseContext context){
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<News>> GetNews(long id){
            var news = await _context.News.FindAsync(id);
            if (news == null) {
                return NotFound();
            }
            return news;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<News>>> GetAllNews(){
            return await _context.News.ToArrayAsync();
        }
        [HttpPost]
        public async Task<ActionResult<News>> PostNews(News news){
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetNews", new{id = news.id}, news);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<News>> UpdateNews(long id, News news){
            if (id != news.id) {
                return BadRequest();
            }
            _context.Entry(news).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetNews", new { id = news.id }, news);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<News>> DeleteNews(long id){
            var news = await _context.News.FindAsync(id);
            if (news == null) {
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return news;
        }
        [HttpGet("filter/{userId}/{categoryId}")]
        public async Task<ActionResult<IEnumerable<News>>> CategoryFilter(long userId,long categoryId){
            var results = from news in _context.News select news;
            return await results.Where((news) => news.userId == userId && news.categoryId == categoryId).ToArrayAsync();
        }
    }
}