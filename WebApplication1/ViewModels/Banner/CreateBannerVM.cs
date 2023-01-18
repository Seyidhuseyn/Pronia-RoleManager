namespace WebApplication1.ViewModels
{
    public class CreateBannerVM
    {
        public string PrimaryTitle { get; set; }
        public string SecondaryTitle { get; set; }
        public IFormFile Image { get; set; }
        public int Order { get; set; }
    }
}
