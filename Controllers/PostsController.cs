using AzureSqlVectorSearch.AzureOpenAi;
using AzureSqlVectorSearch.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzureSqlVectorSearch.Controllers;

public class PostsController : ControllerBase
{
    [HttpGet("/search")]
    public async Task<ActionResult<List<Post>>> Search(
        [FromQuery] string query,
        [FromServices] BloggingContext context,
        [FromServices] AzureOpenAiEmbeddingClient embeddingClient)
    {
        float[] vector = await embeddingClient.GetEmbedding(query);
        
        var posts = await context.Posts
            .OrderBy(p => EF.Functions.VectorDistance("cosine", p.Embedding, vector))
            .Select(p => new {p.Title, Distance = EF.Functions.VectorDistance("cosine", p.Embedding, vector)})
            .ToListAsync();

        return Ok(posts);
    }
}