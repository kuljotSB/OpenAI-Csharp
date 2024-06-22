using Azure;
using Azure.AI.OpenAI;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

public static class classGlobals{
 private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
 public static readonly string endpoint = config["OPENAI_ENDPOINT"];
 public static readonly string key = config["OPENAI_KEY"];
 public static readonly string chat_engine = config["OPENAI_CHAT_MODEL"];

}

public static class Program {
    public static void Main()
    {
        Console.WriteLine("The GPT Engine will summarise the text contained in the content.txt file");
        string file_path = "../summarisation/content.txt";
        string file_content = File.ReadAllText(file_path);

        Console.WriteLine("The content of the file is: ");
        Console.WriteLine(file_content);

        string system_message = "You are meant to summarise text";
        string user_message = $"Can you summarise this text in bullet points? {file_content}";

        OpenAIClient client = new OpenAIClient(new Uri(classGlobals.endpoint), new AzureKeyCredential(classGlobals.key));
        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages = {
                new ChatRequestSystemMessage(system_message),
                new ChatRequestUserMessage(user_message)
            },
            DeploymentName = classGlobals.chat_engine
            };

            ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);
            string final_response = response.Choices[0].Message.Content;
            Console.WriteLine("The summarised text is: ");
            Console.WriteLine("------------------------XXXXXX--------------------");
            Console.WriteLine(final_response);
        }
    }
