using Data.Model;

namespace Data.Repository
{
    public interface IBuyerRepository
    {
        public Task<IEnumerable<BuyerProduct>> AllProduct();
    }
}