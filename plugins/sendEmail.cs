#pragma warning disable SKEXP0050

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

/// <summary>
/// A plugin that sends an email to the customer with the booking details.
/// /// </summary>
public class SendEmailPlugin //JM+
{
    [KernelFunction("web_send_email")]
    [Description("send the booking confirmation to the customer's email address.")]
    [return: Description("Success status")]
    public async Task<string> WebSendEmail([Description("The email address")] string email_address,
                                        [Description("The email details")] string email_details)
    {
        // CHALLENGE 2.4
        return "success";
    }
}
