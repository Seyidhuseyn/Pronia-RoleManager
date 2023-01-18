using WebApplication1.ViewModels.Basket;

namespace WebApplication1.ViewModels
{
    public class BasketVM
    {
        public ICollection<FruitBasketItemVM> Fruits { get; set; }
        public double TotalPrice { get; set; }
    }
}
