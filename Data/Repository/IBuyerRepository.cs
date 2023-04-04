using Data.Model;

namespace Data.Repository
{
    public interface IBuyerRepository
    {
        Task<IEnumerable<string>> AllProduct();
    }
}