using Discount.Grpc.Mapper;
using Discount.Grpc.Repositories;
using Discount.Grpc.Seeding;
using Discount.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DiscountDbConnection");
var dataSeeder = new DataSeeder(connectionString);
dataSeeder.Seed();

/* Register custom service */
builder.Services.AddAutoMapper(typeof(DiscountProfile));
builder.Services.AddTransient<IDiscountRepository, DiscountRepository>();
// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapGrpcService<DiscountService>();

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
