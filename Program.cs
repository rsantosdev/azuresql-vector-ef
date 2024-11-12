
using AzureSqlVectorSearch.AzureOpenAi;
using AzureSqlVectorSearch.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureSqlVectorSearch;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddDbContext<BloggingContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), 
                o=> o.UseVectorSearch());
        });

        builder.Services.AddSingleton<AzureOpenAiEmbeddingClient>();

        var app = builder.Build();
        app.MapControllers();

        await DbMigrator.MigrateAsync(app.Services);
        await app.RunAsync();
    }
}