using Discount.Api.Repositories;
using Discount.Api.Seeding;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DiscountDbConnection");
        var dataSeeder = new DataSeeder(connectionString);
        dataSeeder.Seed();
        /* Register system service */
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        /* Register custom service */
        builder.Services.AddTransient<IDiscountRepository, DiscountRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}