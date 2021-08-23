
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProyectoWeb2.Models;
using System.Net.Http;
using System.Xml;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoWeb2.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")] 
    public class NewsSourcesController: ControllerBase
    {
        private readonly DatabaseContext _context;

        public NewsSourcesController(DatabaseContext context){
            _context = context;
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsSource>> GetSource(long id){
            var source = await _context.NewsSources.FindAsync(id);
            if (source == null) {
                return NotFound();
            }
            source.user = await _context.Users.FindAsync(source.userId);
            source.category = await _context.Categorys.FindAsync(source.categoryId);
            return source;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsSource>>> GetSources(){
            return await _context.NewsSources.ToArrayAsync();
        }

        [HttpPost]
        public async Task<ActionResult<NewsSource>> PostSource(NewsSource newsSource){
            _context.NewsSources.Add(newsSource);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetSource", new{id = newsSource.id}, newsSource);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<NewsSource>> DeleteSource(long id){
             var source = await _context.NewsSources.FindAsync(id);
            if (source == null) {
                return NotFound();
            }

            _context.NewsSources.Remove(source);
            await _context.SaveChangesAsync();
            return source;
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<NewsSource>> UpdateSource(long id, NewsSource newsSource){
            if (id != newsSource.id) {
                return BadRequest();
            }
            _context.Entry(newsSource).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetSource", new { id = newsSource.id }, newsSource);
        }

        [HttpGet("userSources/{id}")]
        public async Task<ActionResult<IEnumerable<NewsSource>>> getUserSources(long id){
            var results = await _context.NewsSources.Where((source) => source.userId == id).ToListAsync();
            List<NewsSource> sources = new List<NewsSource>();
            if(results.Count > 0){
            
                results.ForEach((source) => {
                    source.category =  _context.Categorys.Find(source.categoryId);
                    sources.Add(source);
                });
            }
            return sources;
        }

        [HttpGet("news/{userId}")]
        public async Task<ActionResult<IEnumerable<News>>> Charge(long userId){
            var results = from categorys in _context.Categorys select categorys;
            List<News> oldNews = await _context.News.Where((news) => news.userId == userId).ToListAsync();

            foreach (News item in oldNews)
            {
                _context.News.Remove(item);
            }
            
            List<NewsSource> sources = await _context.NewsSources.Where((source) => source.userId == userId).ToListAsync();

            var httpClient = HttpClientFactory.Create();
            List<News> news = new List<News>();
            
            foreach (NewsSource source in sources)
            {
                List<XmlNode> itemsList = getNodes(source,httpClient,results).Result;
                foreach (XmlNode item in itemsList)
                {
                    var title = item.SelectSingleNode("title").InnerText;
                    var description = item.SelectSingleNode("description").InnerText.Split("<");
                    var link = item.SelectSingleNode("link").InnerText;
                    var date = DateTime.Parse(item.SelectSingleNode("pubDate").InnerText);
                    var categoryText = item.SelectSingleNode("category").InnerText;
                    var category = results.Where((x)=> x.name == categoryText).Single();
                    News newNotice = new News();
                    newNotice.title = title;
                    newNotice.description = description[0].Length > 200 ? description[0].Substring(0,200) : description[0];
                    newNotice.link = link;
                    newNotice.date = date;
                    newNotice.categoryId = category.id;
                    newNotice.userId = userId;
                    newNotice.newsSourceId = source.id;
                    news.Add(newNotice);
                }
                
                    
            }
            foreach (var item in news)
                {
                    await _context.AddAsync(item);
                }
                await _context.SaveChangesAsync();

            return await _context.News.Where((x) => x.userId == userId).OrderBy((news) => news.date).ToListAsync();
        }

        async Task<List<XmlNode>> getNodes(NewsSource source, HttpClient httpClient, IQueryable<Category> results){
            var doc = new XmlDocument();

            string url = source.url;
            
            var data = await httpClient.GetStringAsync(url);
            doc.LoadXml(data);
            
            XmlNodeList items = doc.GetElementsByTagName("item");
            List<XmlNode> temporalList = new List<XmlNode>();
            
            foreach (XmlNode item in items)
            {
                temporalList.Add(item);
            }

            List<XmlNode> itemsList = temporalList.ToList();
            foreach (XmlNode item in temporalList)
            {
                var categoryText = item.SelectSingleNode("category").InnerText;
                var category = results.Where((x) => x.name == categoryText);
                if(category.Count() == 0){
                    itemsList.Remove(item);
                }
            }
            return itemsList;
        }

        
       

    }
}