using Data.Model;

namespace Data.Repository
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly BuyerProductContext _context;

        public BuyerRepository(BuyerProductContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BuyerProduct>> AllProduct()
        {
            throw new NotImplementedException();
        }
    }
}