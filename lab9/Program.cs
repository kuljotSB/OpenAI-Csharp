using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using Newtonsoft.Json;
using System.IO;

public class Globals 
{
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string openai_endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string openai_key = config["OPENAI_KEY"];
    public static readonly string openai_deploymentName = config["OPENAI_CHAT_MODEL"];
    public static readonly string whisperModelName = config["WHISPER_MODEL_NAME"];

}

public class Program {
    public async static Task getResponse(string user_prompt) {
        OpenAIClient client = new OpenAIClient(new Uri(Globals.openai_endpoint), new AzureKeyCredential(Globals.openai_key));
        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions() {
            Messages = {
                new ChatRequestSystemMessage("You are a helpful AI assistant"),
                new ChatRequestUserMessage(user_prompt)
            },
            DeploymentName = Globals.openai_deploymentName
        };
        ChatCompletions chatCompletionsResponse = client.GetChatCompletions(chatCompletionsOptions);
        Console.WriteLine(chatCompletionsResponse.Choices[0].Message.Content);
    }
    public async static Task Main()
    {
    string final_url = $"{Globals.openai_endpoint}/openai/deployments/{Globals.whisperModelName}/audio/transcriptions?api-version=2023-09-01-preview";
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("api-key", Globals.openai_key);

    string filePath = "../lab9/voice.wav";

    var content = new MultipartFormDataContent();
    content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(filePath)), "file", "voice.wav");

    var response = await client.PostAsync(final_url, content);

      if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(responseBody);
                string text = json.text;
                Console.WriteLine("fetching Response from the chat completions model");
                Console.WriteLine("--------------------XXXXXXXXXX---------------");
                await Program.getResponse(text);
                
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

}
}