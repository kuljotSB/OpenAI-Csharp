using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text;
using System.Text.Json;
using System;
using System.IO;

public static class Globals{
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string? endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string? key = config["OPENAI_KEY"];
    public static readonly string? embeddingModel = config["OPENAI_EMBEDDING_MODEL"];

}

public class Program {
    public static void Main() {
        Console.WriteLine("Enter your query");
        string user_query = Console.ReadLine();

        OpenAIClient client = new OpenAIClient(new Uri(Globals.endpoint), new AzureKeyCredential(Globals.key));

        EmbeddingsOptions embeddingOptions = new EmbeddingsOptions() {
            DeploymentName = Globals.embeddingModel,
            Input = { user_query }
        };

        var getEmbeddings = client.GetEmbeddings(embeddingOptions);

        Console.WriteLine("Generating first 10 Embeddings:");
        Console.WriteLine("----------");

        int count=0;

        foreach (float item in getEmbeddings.Value.Data[0].Embedding.ToArray())
            {
                if(count<10){
                Console.WriteLine(item);
                count=count+1;
            }
            else
            {
                break;
            }
            }

        Console.WriteLine("----------");
        Console.WriteLine("Embeddings generated successfully.");
    }
}