using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string PrimaryTitle { get; set; }
        public string SecondaryTitle { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
