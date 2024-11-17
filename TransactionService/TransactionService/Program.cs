using Microsoft.EntityFrameworkCore;
using TransactionService.Data;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<TransactionDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add HttpClient service for communication with UserService
        builder.Services.AddHttpClient<IUserService, UserService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5273"); // Replace with actual UserService URL
            client.Timeout = TimeSpan.FromSeconds(30);
        });


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}