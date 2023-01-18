using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MinLength(2, ErrorMessage = "uzunluq 2-den boyuk olmalidir."), MaxLength(20, ErrorMessage = "uzunluq 20-den kicik olmalidir.")]
        public string Name { get; set; }
        public string Photo { get; set; }
        public List<Product> products { get; set; }

    }
}
