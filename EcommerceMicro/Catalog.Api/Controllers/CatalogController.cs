using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.Api.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class CatalogController : ControllerBase
	{
		private readonly IProductRepository productRepository;
		private readonly ILogger<CatalogController> logger;

		public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
		{
			this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		public async Task<IActionResult> GetProducts() 
		{
			var products = await productRepository.GetProducts();
			return Ok(products);
		}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await productRepository.GetProductById(id);
			if (product==null)
			{
				logger.LogError($"Product with log Id {id} not found!");
				return NotFound();
			}
            return Ok(product);
        }

        [HttpGet]
		[Route("[action]/{category}", Name = "GetProductByCategory")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await productRepository.GetProductsByCategory(category);
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await productRepository.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await productRepository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await productRepository.DeleteProduct(id));
        }
    }
}
