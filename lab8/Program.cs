using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using Azure.AI.DocumentIntelligence;

public class Globals 
{
    private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    public static readonly string? openai_endpoint = config["OPENAI_ENDPOINT"];
    public static readonly string? openai_key = config["OPENAI_KEY"];
    public static readonly string? openai_deploymentName = config["OPENAI_CHAT_MODEL"];
    public static readonly string? doc_int_endpoint = config["DOC_INT_ENDPOINT"];
    public static readonly string? doc_int_key = config["DOC_INT_KEY"];
}

public class Program {

    public static void getResponse(string prompt) {
        OpenAIClient client = new OpenAIClient(new Uri(Globals.openai_endpoint), new AzureKeyCredential(Globals.openai_key));

        ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions() {
            Messages = {
                new ChatRequestSystemMessage("You are a helpful AI assistant"),
                new ChatRequestUserMessage(prompt)
            },
            DeploymentName = Globals.openai_deploymentName
        };

        ChatCompletions chatCompletionsResponse = client.GetChatCompletions(chatCompletionsOptions);
        Console.WriteLine(chatCompletionsResponse.Choices[0].Message.Content);

    }
    public static async Task Main() {
        DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(Globals.doc_int_endpoint), new AzureKeyCredential(Globals.doc_int_key));

        Uri invoiceUri = new Uri ("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf");

        AnalyzeDocumentContent content = new AnalyzeDocumentContent()
        {
            UrlSource = invoiceUri
        };

        Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", content);

        AnalyzeResult result = operation.Value;

        string vendorName = string.Empty;
        string vendorAddress = string.Empty;
        string invoiceDate = string.Empty;
        string subtotal = string.Empty;

        for (int i = 0; i < result.Documents.Count; i++)
        {
            

            AnalyzedDocument document = result.Documents[i];

            if (document.Fields.TryGetValue("VendorName", out DocumentField vendorNameField)
                && vendorNameField.Type == DocumentFieldType.String)
            {
                vendorName = vendorNameField.ValueString;
                
            }

            if (document.Fields.TryGetValue("VendorAddress", out DocumentField vendorAddressField)
                && vendorAddressField.Type == DocumentFieldType.String)
            {
                vendorAddress = vendorAddressField.ValueString;
            }

            if (document.Fields.TryGetValue("InvoiceDate", out DocumentField invoiceDateField)
                && invoiceDateField.Type == DocumentFieldType.String)
            {
                invoiceDate = invoiceDateField.ValueString;
            }
            if (document.Fields.TryGetValue("SubTotal", out DocumentField subTotalField)
                && subTotalField.Type == DocumentFieldType.String)
            {
                subtotal = subTotalField.ValueString;
            }

            }
        
          string prompt = @$"You have been provided the details of an invoice. generate a csv file based upon the following details given to you:"
            + $"\nVendor Name: {vendorName}"
            + $"\nVendor Address: {vendorAddress}"
            + $"\nInvoice Date: {invoiceDate}"
            + $"\nSubtotal: {subtotal}";

         Program.getResponse(prompt);

}
}