using Azure;
using Azure.Search.Documents;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using System;
using Azure.Core.GeoJson;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.AI.OpenAI;
public static class Globals
{
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string? endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string? key = config["OPENAI_KEY"];
    public static readonly string? deploymentName = config["OPENAI_CHAT_MODEL"];
    public static readonly string? searchEndpoint = config["SEARCH_ENDPOINT"];
    public static readonly string? searchKey = config["SEARCH_KEY"];
    public static readonly string? indexName = config["SEARCH_INDEX_NAME"];



}

public class Program
{
  public static void Main(){
  Console.WriteLine("enter your user_query");
  string user_query = Console.ReadLine();
  SearchClient searchClient = new SearchClient(new Uri(Globals.searchEndpoint), Globals.indexName, new AzureKeyCredential(Globals.searchKey));
  SearchResults<SearchDocument> searchResults = searchClient.Search<SearchDocument>($"{user_query}");
  int count=0;
  string keyphrases="";

  foreach (SearchResult<SearchDocument> result in searchResults.GetResults())
  {
    if(count==0) {
    SearchDocument doc = result.Document;
    dynamic data = JsonConvert.DeserializeObject(doc.ToString());
    var keyPhrasesList = data.keyphrases;
    foreach (var keyphrase in keyPhrasesList)
    {
      keyphrases=keyphrases+keyphrase+",";
    }
    count=count+1;
    }
  }

  string response = getResponse(keyphrases);
  Console.WriteLine(response);

  

}

public static string getResponse(string keyhrase) {
  string keyphrases = keyhrase;
  OpenAIClient client = new OpenAIClient(new Uri(Globals.endpoint), new AzureKeyCredential(Globals.key));

  string system_message = "You are a helpful AI assistant";

  string user_message = $@"I am providing you with the list of important keywords taken from an engineering entrance exam question paper.
  By analyzing these keywords, you can get an idea of the topics that are important for the exam.
  the keywords are: {keyphrases}";

  ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions() 
  {
    Messages = {
      new ChatRequestSystemMessage(system_message),
      new ChatRequestUserMessage(user_message)
    },
    DeploymentName = Globals.deploymentName,
  };

  ChatCompletions chatCompletions = client.GetChatCompletions(chatCompletionsOptions);
  string response = chatCompletions.Choices[0].Message.Content;
  return response;
}
}