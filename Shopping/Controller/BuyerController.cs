using Microsoft.AspNetCore.Mvc;
using Service;

namespace Shopping.Controller
{
    public class BuyerController
    {
        private readonly IBuyerService _buyerService;
        public BuyerController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        [HttpGet]
        public Task<ActionResult> AllProduct()
        {
            throw new NotImplementedException();
        }
    }
}