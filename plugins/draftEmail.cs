#pragma warning disable SKEXP0050

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

/// <summary>
/// A plugin that searches the web to get information that contains the next match date and time for the a passed football team.
/// https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/using-data-retrieval-functions-for-rag
/// </summary>
public class DraftEmailPlugin //JM+
{
    [KernelFunction("web_draft_email")]
    [Description("draft the booking confirmation to the customer's email address.")]
    [return: Description("The body of the email.")]
    public async Task<string> WebDraftEmail([Description("The email address")] string email_address,
                                        [Description("The booking details")] BookingModel booking_details)
    {
        // CHALLENGE 2.4
        // Write a native function that sends an email to the customer with the booking details.
        var kernel = Kernel.CreateBuilder().Build();
        
        var bingConnector = new BingConnector("<your-bing-service-key>");
        kernel.ImportPluginFromObject(new WebSearchEnginePlugin(bingConnector), "bing");

        var function = kernel.Plugins["bing"]["search"];
        var bingResult = await kernel.InvokeAsync(function, new() { ["query"] = "Send email to "+ email_address + " with the booking details: " + booking_details });
        return bingResult.ToString();
    }
}
