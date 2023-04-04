using Data.Model;

namespace Data.Repository
{
    public interface IBuyerRepository
    {
        Task<IEnumerable<BuyerProduct>> AllProduct();
    }
}