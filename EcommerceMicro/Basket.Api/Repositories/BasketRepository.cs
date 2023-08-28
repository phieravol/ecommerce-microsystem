using Basket.Api.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.Api.Repositories
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDistributedCache redisCache;

		public BasketRepository(IDistributedCache redisCache)
		{
			this.redisCache = redisCache;
		}

		/// <summary>
		/// Delete basket by Username
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public async Task DeleteBasket(string userName)
		{
			await redisCache.RemoveAsync(userName);
		}

		/// <summary>
		/// Get Basket by Username
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public async Task<ShoppingCart> GetBasket(string userName)
		{
			//get basket as string type by username
			var rawBasket = await redisCache.GetStringAsync(userName);

			//return empty basket when string is empty
			if (String.IsNullOrEmpty(rawBasket))
			{
				return null;
			}

			//convert basket from json into object
			var basket = JsonConvert.DeserializeObject<ShoppingCart>(rawBasket);
			return basket;
		}

		/// <summary>
		/// Update Basket when shopping-item changeds 
		/// </summary>
		/// <param name="basket"></param>
		/// <returns></returns>
		public Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
		{
			//Convert basket into json format
			var jsonBasket = JsonConvert.SerializeObject(basket);
			//Update basket by key and value
			redisCache.SetStringAsync(basket.UserName, jsonBasket);
			//Return current basket
			return GetBasket(basket.UserName);
		}
	}
}
