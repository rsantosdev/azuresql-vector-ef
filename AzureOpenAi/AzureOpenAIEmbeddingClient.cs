using Azure.AI.OpenAI;
using System.ClientModel;

namespace AzureSqlVectorSearch.AzureOpenAi;

public class AzureOpenAiEmbeddingClient
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;

    public AzureOpenAiEmbeddingClient(IConfiguration configuration)
    {
        _client = new AzureOpenAIClient(new Uri(configuration["AzureOpenAi:Endpoint"]), new ApiKeyCredential(configuration["AzureOpenAi:ApiKey"]));
        _deploymentName = configuration["AzureOpenAi:DeploymentName"];
    }

    public async Task<float[]> GetEmbedding(string text, int dimensions = 1536)
    {
        var embeddingClient = _client.GetEmbeddingClient(_deploymentName);

        var embedding = await embeddingClient.GenerateEmbeddingAsync(text, new() { Dimensions = dimensions });

        var vector = embedding.Value.ToFloats().ToArray();
        if (vector.Length != dimensions)
        {
            throw new Exception($"Expected {dimensions} dimensions, but got {vector.Length}");
        }

        return vector;
    }
}