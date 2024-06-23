using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using Microsoft.CognitiveServices.Speech;
using System.Threading;
using Microsoft.CognitiveServices.Speech.Audio;

public static class Globals {
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string? openAIKey = config["OPENAI_KEY"];
    public static readonly string? openAIEndpoint = config["OPENAI_ENDPOINT"];
    public static readonly string? openAIModel = config["OPENAI_CHAT_MODEL"];
    public static readonly string? speechKey = config["SPEECH_KEY"];
    public static readonly string? speechRegion = config["SPEECH_REGION"];

}

public class Program {

    public  static string GetOpenAIResponse(string user_input) {
       OpenAIClient client = new OpenAIClient(new Uri(Globals.openAIEndpoint), new AzureKeyCredential(Globals.openAIKey));
       
       string system_message = "You are a helpful AI assistant";
       string user_message = user_input;

       ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions(){
        Messages = {
            new ChatRequestSystemMessage(system_message),
            new ChatRequestUserMessage(user_message)
        },
        DeploymentName = Globals.openAIModel,
       };

       ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);

       string chatResponse = response.Choices[0].Message.Content;

       return chatResponse;

    }
    public async static Task Main() {
        var speechConfig = SpeechConfig.FromSubscription(Globals.speechKey, Globals.speechRegion);
        Console.WriteLine("Ready to use speech service in " + speechConfig.Region);

        // Configure speech recognition
        using AudioConfig audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        Console.WriteLine("Speak now...");
        
        string command="";

        // Process speech input
        SpeechRecognitionResult speech = await speechRecognizer.RecognizeOnceAsync();
        if (speech.Reason == ResultReason.RecognizedSpeech)
        {
            command = speech.Text;
            Console.WriteLine(command);
            string response = Program.GetOpenAIResponse(command);
            Console.WriteLine(response);
        }
        else
        {
            Console.WriteLine(speech.Reason);
            if (speech.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(speech);
                Console.WriteLine(cancellation.Reason);
                Console.WriteLine(cancellation.ErrorDetails);
            }
        }


    }
}
