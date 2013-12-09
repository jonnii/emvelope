using System.Web.Http;
using Emvelope.Integration.Models;

namespace Emvelope.Integration.Controllers
{
    public class ProductsController : ApiController
    {
        public PagedEnumerable<Product> Get()
        {
            var results = new[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Cat Food",
                    InventoryCode = "CF-2343-YUM"
                },
                new Product
                {
                    Id = 1,
                    Name = "Cat Food",
                    InventoryCode = "CF-2343-YUM"
                },
                new Product
                {
                    Id = 1,
                    Name = "Cat Food",
                    InventoryCode = "CF-2343-YUM"
                }
            };

            return new PagedEnumerable<Product>(results)
            {
                Page = 3,
                ItemsPerPage = 20,
                TotalPages = 150
            };
        }

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
