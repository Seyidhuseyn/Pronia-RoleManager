using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Sponsor
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}