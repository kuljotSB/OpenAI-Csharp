using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Azure;
using Azure.AI.OpenAI;


public class GetWeatherFunction
{
    static public string Name = "get_current_weather";

    // Return the function metadata
    static public FunctionDefinition GetFunctionDefinition()
    {
        return new FunctionDefinition()
        {
            Name = Name,
            Description = "Get the current weather in a given location",
            Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    location = new
                    {
                        Type = "string",
                        Description = "the exact location whose real-time weather is to be determined",
                    }
                },
                Required = new[] { "location" },
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };
    }
}

public static class Globals
{
     private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string key = config["OPENAI_KEY"];
    public static readonly string model = config["OPENAI_CHAT_MODEL"];

}

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Enter your query");
        
        string user_query = Console.ReadLine();

        OpenAIClient client = new OpenAIClient(new Uri(Globals.endpoint), new AzureKeyCredential(Globals.key));

        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages = {
              new ChatRequestSystemMessage("You are a helpful AI Assistant"),
              new ChatRequestUserMessage(user_query),
            },
            DeploymentName = Globals.model
        };

        FunctionDefinition getWeatherFunctionDefinition = GetWeatherFunction.GetFunctionDefinition();
        
        chatCompletionsOptions.Functions.Add(getWeatherFunctionDefinition);

        ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);

        ChatChoice responseChoice = response.Choices[0];

        string functionName = responseChoice.Message.FunctionCall.Name;





        }

    }

    
