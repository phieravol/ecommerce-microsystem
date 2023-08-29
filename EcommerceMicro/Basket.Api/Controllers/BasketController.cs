using Basket.Api.Entities;
using Basket.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class BasketController : ControllerBase
	{
		private readonly IBasketRepository repository;

		public BasketController(IBasketRepository repository)
		{
			this.repository = repository;
		}

		[HttpGet("{username}")]
		public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
		{
			var basket = await repository.GetBasket(username);
			return Ok(basket ?? new ShoppingCart(username));
		}

		[HttpPost]
		public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
		{
			var currentBasket = await repository.UpdateBasket(basket);
			return Ok(currentBasket);
		}

		[HttpDelete("{username}")]
		public async Task<IActionResult> DeleteBasket(string username)
		{
			await repository.DeleteBasket(username);
			return NoContent();
		}
	}
}
