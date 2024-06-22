using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json;

public static class Globals
{
     private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string key = config["OPENAI_KEY"];
    public static readonly string model = config["OPENAI_CHAT_MODEL"];

    public static readonly string openweatherAPIKey = config["OPENWEATHER_API_KEY"];

}

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

    public static async Task<string> get_current_weather(string location)
    {
        string url = $"http://api.openweathermap.org/geo/1.0/direct?q={location}&limit=1&appid={Globals.openweatherAPIKey}";

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);

        var response  = await client.GetAsync(url);

        string stringResponse = await response.Content.ReadAsStringAsync();

       dynamic coordinateObject = JsonConvert.DeserializeObject(stringResponse);

       double latitude = coordinateObject[0].lat;
       double longitude = coordinateObject[0].lon;

       string url2 = $"http://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={Globals.openweatherAPIKey}";

       var final_response = await client.GetAsync(url2);

       string finalResponseString = await final_response.Content.ReadAsStringAsync();

       dynamic finalResponseObject = JsonConvert.DeserializeObject(finalResponseString);

       string weatherCondition = finalResponseObject.weather[0].description;

       return weatherCondition;



        
    }

    
}



public static class Program
{
    public static async Task Main()
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

        string unvalidatedFunctionArguments = responseChoice.Message.FunctionCall.Arguments;

        dynamic functionArgument = JsonConvert.DeserializeObject(unvalidatedFunctionArguments);

        if(functionName=="get_current_weather")
        {
            var finalResponse = await GetWeatherFunction.get_current_weather(functionArgument.location.ToString());
            Console.WriteLine(finalResponse);  
            
        }
    }
}


        





        

    
