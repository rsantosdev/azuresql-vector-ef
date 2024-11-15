﻿using System.Text.Json;
using AzureSqlVectorSearch.AzureOpenAi;
using Microsoft.EntityFrameworkCore;

namespace AzureSqlVectorSearch.Data;

public static class DbMigrator
{
    public static async Task MigrateAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BloggingContext>();
        var embeddingClient = scope.ServiceProvider.GetRequiredService<AzureOpenAiEmbeddingClient>();
        await context.Database.MigrateAsync();

        var sampleBlog = await context.Blogs.FirstOrDefaultAsync();
        if (sampleBlog != null)
        {
            return;
        }

        var blog = new Blog { Name = "AzureBrasil.cloud Blog", Url = "https://azurebrasil.cloud" };
        await context.Blogs.AddAsync(blog);
        await context.SaveChangesAsync();

        var posts = JsonSerializer.Deserialize<List<SavedPost>>(BlogPosts);
        foreach (var post in posts)
        {
            await context.Posts.AddAsync(new Post
            {
                Title = post.Title,
                Content = post.Content,
                BlogId = blog.BlogId,
                Embedding = await embeddingClient.GetEmbedding(post.Content)
            });
        }
        await context.SaveChangesAsync();
    }

    private const string BlogPosts = @"
        [
            {
                ""Title"": ""My first EF Core app!"",
                ""Content"": ""I wrote an app using EF Core!""
            },
            {
                ""Title"": ""Vectors with Azure SQL and EF Core"",
                ""Content"": ""You can use and store vectors easily Azure SQL and EF Core""
            },
            {
                ""Title"": ""EFCore.SqlServer.VectorSearch in PreRelease"",
                ""Content"": ""The NuGet package EFCore.SqlServer.VectorSearch is now available in PreRelease! With this package you can use vector search functions in your LINQ queries.""
            },
            {
                ""Title"": ""SQL Server Best Practices"",
                ""Content"": ""Here are some best practices for using SQL Server in your applications.""
            },
            {
                ""Title"": ""Python and Flask"",
                ""Content"": ""Learn how to build a web app using Python and Flask!""
            },
            {
                ""Title"": ""Django for REST APIs"",
                ""Content"": ""Create a REST API using Django!""
            },
            {
                ""Title"": ""JavaScript for Beginners"",
                ""Content"": ""Learn JavaScript from scratch!""
            },
            {
                ""Title"": ""Node vs Rust"",
                ""Content"": ""Which one should you choose for your next project?""
            },
            {
                ""Title"": ""Pizza or Focaccia?"",
                ""Content"": ""What's the difference between pizza and focaccia. Learn everything you need to know!""
            },
            {
                ""Title"": ""Chocolate Eggs for your next dessert"",
                ""Content"": ""Try this delicious recipe for chocolate eggs!""
            }
        ]
    ";
}