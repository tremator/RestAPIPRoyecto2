using ProyectoWeb2.Models;

namespace ProyectoWeb2Api.Models
{
    public class Tags
    {
        public long id { get; set; }
        public string tag { get; set; }
        public long userId { get; set; }
        public User user { get; set; }
    }
}