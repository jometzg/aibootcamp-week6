#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0060


using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Planning.Handlebars;

// Create a new kernel
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    "<YOUR_DEPLOYMENT_NAME>",
    "<YOUR_AOAI_ENDPOINT>",
    "<YOUR_AOAI_API_KEY>",
    "<SERVICE_ID>");
var kernel = builder.Build();


//-------------------------------
// import plugins
kernel.ImportPluginFromType<SearchPlugin>();
kernel.ImportPluginFromType<ConversationSummaryPlugin>();
var plugins = kernel.ImportPluginFromPromptDirectory("plugins");
var prompts = kernel.ImportPluginFromPromptDirectory("prompts/emails");
//-------------------------------


//-------------------------------
// CHALLENGE 2.1
// https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-native-plugins?pivots=programming-language-csharp
// Write a native function that calls a REST API (e.g. Bing search) to automatically retrieve the day and time of the next [your favorite team 
// and sport] game in order to be integrated in the email.
var web_search_result = await kernel.InvokeAsync("SearchPlugin", 
    "web_football_match_search",
    new() {
        { "footballTeam", "Welling United" }
    });

Console.WriteLine(web_search_result);
//-------------------------------


//-------------------------------
// CHALLENGE 2.1
// https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/
// Write a semantic function that gets the date and time of the next Manchester United football match. 
//The function takes as input web search results for the next Manchester United football match.
var day_and_time = await kernel.InvokeAsync<string>(prompts["date_and_time"],
    new() {
        { "web_search_result", web_search_result }
    }
);

Console.WriteLine(day_and_time);
//--------------------------------


//-------------------------------
// CHALLENGE 2.1
// https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/
// Write a semantic function that generates an excuse email for your boss to avoid work and watch the next [your favorite team and sport] game. 
//The function takes as input the day and time of the game, which you provide manually.
var excuse_email = await kernel.InvokeAsync<string>(prompts["excuses"],
    new() {
        { "day_and_time", day_and_time }
    }
);

Console.WriteLine(excuse_email);
//--------------------------------


//--------------------------------
// CHALLENGE 2.2+
// Create an execution plan using Intelligent Planners
// https://learn.microsoft.com/en-us/training/modules/use-intelligent-planners/1-introduction
var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

string footballTeam = "Welling United";
string goal = @$"The user's favourite football team is ${footballTeam}. 
    Create an excuse email for the user's boss so that they can be absent 
    from work on the date and time of the next scheduled ${footballTeam} match.";

var plan = await planner.CreatePlanAsync(kernel, goal);
Console.WriteLine(plan);

var result = await plan.InvokeAsync(kernel);

Console.WriteLine(result);
//-------------------------------