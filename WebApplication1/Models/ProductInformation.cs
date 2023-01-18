namespace WebApplication1.Models
{
    public class ProductInformation
    {
        public int Id { get; set; }
        public string Shipping { get; set; }
        public string AboutReturnRequest { get; set; }
        public string Guarantee { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
