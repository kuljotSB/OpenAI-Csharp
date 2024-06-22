using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;


public static class Globals
{
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string key = config["OPENAI_KEY"];
    public static readonly string model = config["OPENAI_CHAT_MODEL"];
    
    public static readonly string contentSafetyKey = config["CONTENT_SAFETY_KEY"];

    public static readonly string contentSafetyEndpoint = config["CONTENT_SAFETY_ENDPOINT"];

}

public static class Program
{
    public static async Task Main()
    {
        string user_input;
        Console.WriteLine("Enter you query: ");
        user_input = Console.ReadLine();

        var url = $"https://kuljotcontentsafety.cognitiveservices.azure.com/contentsafety/text:analyze?api-version=2023-10-01";

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(url);
        
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", $"{Globals.contentSafetyKey}");
        
        var body = new {
            text = $"{user_input}"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(body);

        var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

          
        var responseContent = await response.Content.ReadAsStringAsync();
        dynamic data = JsonConvert.DeserializeObject(responseContent);
        Console.WriteLine(data);
            
        

        int responseThreshold = 2;

        int hateSeverity = data.categoriesAnalysis[0].severity;
        int selfHarmSeverity = data.categoriesAnalysis[1].severity;
        int SexualSeverity = data.categoriesAnalysis[2].severity;
        int violenceSeverity = data.categoriesAnalysis[3].severity;

        if(hateSeverity > responseThreshold)
        {
            Console.WriteLine("Hate speech detected");
        }
        else if(selfHarmSeverity > responseThreshold)
        {
            Console.WriteLine("Self harm detected");
        }
        else if(SexualSeverity > responseThreshold)
        {
            Console.WriteLine("Sexual content detected");
        }
        else if(violenceSeverity > responseThreshold)
        {
            Console.WriteLine("Violence detected");
        }
        else
        {
            Console.WriteLine("Sending message to the chat engine");
            var chatResponse = await getResponse(user_input);
            string stringResponse = chatResponse.Choices[0].Message.Content;
            Console.WriteLine(stringResponse);

        }



        
    }

    public static async Task<ChatCompletions> getResponse(string user_message)
    {
        OpenAIClient client = new OpenAIClient(new Uri(Globals.endpoint), new AzureKeyCredential(Globals.key));

        string system_message = "You are a helpful AI Assistant";

        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages = {
                new ChatRequestSystemMessage(system_message),
                new ChatRequestUserMessage(user_message)
            },
            DeploymentName = Globals.model

        };

        ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);
        return response;
    }
}