using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services
{
	public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
	{
		private readonly ILogger<DiscountService> logger;
		private readonly IDiscountRepository repository;
		private readonly IMapper mapper;

		public DiscountService(ILogger<DiscountService> logger, IDiscountRepository repository, IMapper mapper)
		{
			this.logger = logger;
			this.repository = repository;
			this.mapper = mapper;
		}

		public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
		{
			var coupon = await repository.GetDiscount(request.ProductName);
			if (coupon == null)
			{
				throw new RpcException(new Status(StatusCode.NotFound, $"Discount with productName={request.ProductName}"));
			}
			logger.LogInformation("Discount is retrieved for ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);
			var couponModel = mapper.Map<CouponModel>(coupon);
			return couponModel;
		}

		public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
		{
			var coupon = mapper.Map<Coupon>(request.Coupon);
			await repository.CreateDiscount(coupon);

			logger.LogInformation("Discount is successfully created. ProductName: {ProductName}", coupon.ProductName);
			var couponModel = mapper.Map<CouponModel>(coupon);
			return couponModel;
		}

		public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
		{
			var coupon = mapper.Map<Coupon>(request.Coupon);
			await repository.UpdateDiscount(coupon);
			logger.LogInformation("Discount is successfully updated. ProductName: {ProductName}", coupon.ProductName);
			var couponModel = mapper.Map<CouponModel>(coupon);
			return couponModel;
		}

		public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
		{
			var deleted = await repository.DeleteDiscount(request.ProductName);
			var response = new DeleteDiscountResponse()
			{
				Success = deleted
			};
			return response;
		}
	}
}
