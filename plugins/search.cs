#pragma warning disable SKEXP0050

using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Bing;

/// <summary>
/// A plugin that searches the web to get information that contains the next match date and time for the a passed football team.
/// https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/using-data-retrieval-functions-for-rag
/// </summary>
public class SearchPlugin
{
    [KernelFunction("web_football_match_search")]
    [Description("Gets information that contains the next match date and time for the a passed football team.")]
    [return: Description("Information that contains the next match date and time for the a passed football team.")]
    public async Task<string> WebSearch([Description("The football team")] string football_team)
    {
        // Write a native function that calls a REST API (e.g. Bing search) to automatically retrieve the day and time of the next [your favorite team 
        // and sport] game in order to be integrated in the email.
        var kernel = Kernel.CreateBuilder().Build();
        
        var bingConnector = new BingConnector("<YOUR_BING_API_KEY>");
        kernel.ImportPluginFromObject(new WebSearchEnginePlugin(bingConnector), "bing");

        var function = kernel.Plugins["bing"]["search"];
        var bingResult = await kernel.InvokeAsync(function, new() { ["query"] = "When is the next "+ football_team + " football match?" });
        return bingResult.ToString();
    }
}