using Basket.Api.Entities;
using Basket.Api.GrpcServices;
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
		private readonly DiscountGrpcService discountGrpcService;

		public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
		{
			this.repository = repository;
			this.discountGrpcService = discountGrpcService;
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
			// TODO: Communicate with Discount.Grpc
			// and Calculate lastest prices of product into shopping cart
			// consume discount Grpc
			foreach (var item in basket.Items)
			{
				var coupon = await discountGrpcService.GetDiscount(item.ProductName);
				item.Price -= coupon.Amount;
			}
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
