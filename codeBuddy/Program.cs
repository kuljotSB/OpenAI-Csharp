//dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.14
using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text;
using System.Text.Json;



public static class classGlobals{
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
 public static readonly string oaiEndpoint = config["OPENAI_ENDPOINT"];
public static readonly string oaiKey = config["OPENAI_KEY"];
 public static readonly string oaiDeploymentName = config["OPENAI_CHAT_MODEL"];
}

public class program
{
    public static void Main()
    {
      string input_value;
      Console.WriteLine("enter 1 for debugging your python code and 2 for generating test cases");
      input_value = Console.ReadLine();

      if(input_value=="1")
      {
        string file_path = "../codeBuddy/factorial.py";
        var file_content = File.ReadAllText(file_path);
        
        
        string system_message = "You are a code buddy that is meant to debug pythonic code";
        string user_message = $"I am getting an error in my code. Can you help me debug it? {file_content}";

        getResponse(classGlobals.oaiEndpoint,classGlobals.oaiKey,system_message,user_message,classGlobals.oaiDeploymentName);
      }
      if(input_value=="2")
      {
        string file_path = "../codeBuddy/function.py";
        var file_content = File.ReadAllText(file_path);
        

        string system_message = "You are a code buddy that is meant to generate test cases for pythonic code";
        string user_message = $"I am getting an error in my code. Can you help me generate test cases for it? {file_content}";

        getResponse(classGlobals.oaiEndpoint,classGlobals.oaiKey,system_message,user_message,classGlobals.oaiDeploymentName);
      }
    }

    public static string getResponse(string endpoint,string key, string system_message, string user_message, string chat_engine)
    {
        OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
        {
            Messages = {
                new ChatRequestSystemMessage(system_message),
                new ChatRequestUserMessage(user_message)
            },
            DeploymentName = chat_engine
        };

        ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);
        string final_response = response.Choices[0].Message.Content;
        Console.WriteLine(final_response);
        return final_response;
    }
}

