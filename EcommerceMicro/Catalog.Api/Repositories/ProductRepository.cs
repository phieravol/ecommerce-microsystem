﻿using Catalog.Api.Data;
using Catalog.Api.Entities;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Catalog.Api.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ICatalogContext context;

		public ProductRepository(ICatalogContext context)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task CreateProduct(Product product)
		{
			await context.Products.InsertOneAsync(product);
		}

		public async Task<bool> DeleteProduct(string id)
		{
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(x => x.Id, id);
            var deleteResult = await context.Products.DeleteOneAsync(filterDefinition);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

		public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
		{
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(x => x.Category, categoryName);
            return await context.Products.Find(filterDefinition).ToListAsync();
        }

		public async Task<Product> GetProductById(string id)
		{
			return await context.Products.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<Product>> GetProductByName(string name)
		{
			FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(x => x.Name, name);
			return await context.Products.Find(filterDefinition).ToListAsync();
		}

		public async Task<IEnumerable<Product>> GetProducts()
		{
			return await context.Products.Find(x => true).ToListAsync();
		}

		public async Task<bool> UpdateProduct(Product product)
		{
			var updateResult = await context.Products.ReplaceOneAsync(filter: g=>g.Id==product.Id, replacement: product);
			return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
		}
	}
}
