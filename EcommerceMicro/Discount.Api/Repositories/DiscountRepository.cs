using Dapper;
using Discount.Api.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Api.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection
                (configuration.GetConnectionString("DiscountDbConnection"));

            var affected =
                await connection.ExecuteAsync
                    ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });

            if (affected == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(configuration.GetConnectionString("DiscountDbConnection"));

            var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
                new { ProductName = productName });

            if (affected == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Get discount infomation base on product name
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public async Task<Coupon> GetDiscount(string productName)
        {
            //Init connection
            using var connection = new NpgsqlConnection(
                configuration.GetConnectionString("DiscountDbConnection"));

            //Get coupon form database
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                "SELECT * FROM COUPON WHERE ProductName = @ProductName", new {ProductName = productName});

            //Create default coupon if no result found
            if (coupon == null)
            {
                Coupon defaultCoupon = new Coupon {
                    ProductName = "No Coupon",
                    Amount = 0,
                    Description = "You have no discount yet",
                };
                return defaultCoupon;
                
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(configuration.GetConnectionString("DiscountDbConnection"));

            var affected = await connection.ExecuteAsync
                    ("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });

            if (affected == 0)
                return false;

            return true;
        }
    }
}
