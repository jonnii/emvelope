using System.Web.Http;
using Emvelope.Integration.Models;

namespace Emvelope.Integration.Controllers
{
    public class ProductsController : ApiController
    {
        public Product Get(int id)
        {
            return new Product
            {
                Id = id,
                Name = "Cat Food",
                InventoryCode = "CF-2343-YUM"
            };
        }
    }
}
