using Data.Model;

namespace Service
{
    public interface IBuyerService
    {
        public Task<IEnumerable<BuyerProduct>> AllProduct();
    }
}