#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0060

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Newtonsoft.Json;


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
kernel.ImportPluginFromType<BookingsPlugin>(); //JM+
kernel.ImportPluginFromType<ConversationSummaryPlugin>();
var prompts = kernel.ImportPluginFromPromptDirectory("C:\\dev\\sk-learn\\aibootcamp-week6\\prompts");
//-------------------------------


//-------------------------------
// execute chained plugins
//var web_search_result = await SearchWebForFootballMatchDateAndTime();
var web_search_result = await SearchRestaurantNearMe();
var website_name = await GetRestaurantWebsite(web_search_result);
var add_booking_result = await WebAddBooking(website_name = website_name );
BookingModel booking = JsonConvert.DeserializeObject<BookingModel>(add_booking_result);
var email_body = await GenerateConfirmationEmail(booking.RestaurantName, 
                                                booking.BookingDate,
                                                booking.BookingTime,
                                                booking.NumberOfPeople,
                                                booking.CustomerName,
                                                booking.CustomerEmail);
//var day_and_time = await GetMatchDateAndTime(web_search_result);
//var excuse_email1 = await GetExcuseEmail(day_and_time);

//execute with functions
//var excuse_email2 = await ExecuteWithFunctions();

//  execute with intelligent planners 
//var excuse_email3 = await ExecuteWithIntelligentPlanners();
//-------------------------------


#region Functions
// CHALLENGE 2.2 JM+
// https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-native-plugins?pivots=programming-language-csharp
// Write a native function that calls a REST API (e.g. Bing search) to automatically retrieve the day and time of the next [your favorite team 
// and sport] game in order to be integrated in the email.
async Task<string> SearchRestaurantNearMe(string cuisine = "Indian", string location = "TF10 7XN")
{
    var web_search_result = await kernel.InvokeAsync<string>("BookingsPlugin", 
    "web_restaurant_search",
    new() {
        { "cuisine", cuisine },
        { "location", location }
    });

    Console.WriteLine(web_search_result);
    return web_search_result;
}

// CHALLENGE 2.2 JM+
// https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/
// Write a semantic function that gets the date and time of the next Manchester United football match. 
//The function takes as input web search results for the next Manchester United football match.
async Task<string> GetRestaurantWebsite(string web_search_result)
{
    var website_name = await kernel.InvokeAsync<string>(prompts["restaurant_website"],
    new() {
            { "restaurant_search_results", web_search_result }
        }
    );

    Console.WriteLine(website_name);
    return website_name;
}

// CHALLENGE 2.3 JM+
// booking a restaurant
async Task<string> WebAddBooking(string restaurant_name = "Dilshad", 
                                string booking_date = "20th September 2022",
                                string booking_time = "19:00",
                                int number_of_people = 2,
                                string customer_name = "John Doe",
                                string customer_email = "john.doe@gmail.com")
{
    var web_add_booking_result = await kernel.InvokeAsync<string>("BookingsPlugin", 
    "web_restaurant_add_booking",
    new() {
        { "restaurant", restaurant_name },
        { "date_booking", booking_date },
        { "time_booking", booking_time },
        { "number_of_people", number_of_people },
        { "customer_name", customer_name },
        { "customer_email", customer_email }
    });
    Console.WriteLine(web_add_booking_result);
    return web_add_booking_result;
}


// CHALLENGE 2.4 JM+
// https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/
// Write a semantic function that generates a confirmation email for a restaurant booking. 
//The function takes as the booking details.
async Task<string> GenerateConfirmationEmail(string restaurant_name,
                                            string booking_date,
                                            string booking_time,
                                            int number_of_people,
                                            string customer_name, 
                                            string customer_email)
{
    var email_body = await kernel.InvokeAsync<string>(prompts["booking_email"],
    new() {
            { "restaurant_name", restaurant_name },
            { "booking_date", booking_date },
            { "booking_time", booking_time },
            { "number_of_people", number_of_people },
            { "customer_name", customer_name}
        }
    );

    Console.WriteLine(email_body);
    return email_body;
}

// CHALLENGE 2.1
// https://learn.microsoft.com/en-us/semantic-kernel/concepts/plugins/adding-native-plugins?pivots=programming-language-csharp
// Write a native function that calls a REST API (e.g. Bing search) to automatically retrieve the day and time of the next [your favorite team 
// and sport] game in order to be integrated in the email.
async Task<string> SearchWebForFootballMatchDateAndTime(string football_team = "Welling United")
{
    var web_search_result = await kernel.InvokeAsync<string>("SearchPlugin", 
    "web_football_match_search",
    new() {
        { "football_team", football_team }
    });

    Console.WriteLine(web_search_result);
    return web_search_result;
}


// CHALLENGE 2.1
// https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/
// Write a semantic function that gets the date and time of the next Manchester United football match. 
//The function takes as input web search results for the next Manchester United football match.
async Task<string> GetMatchDateAndTime(string web_search_result)
{
    var day_and_time = await kernel.InvokeAsync<string>(prompts["date_and_time"],
    new() {
            { "web_search_result", web_search_result }
        }
    );

    Console.WriteLine(day_and_time);
    return day_and_time;
}


// CHALLENGE 2.1
// https://learn.microsoft.com/en-us/training/modules/create-plugins-semantic-kernel/
// Write a semantic function that generates an excuse email for your boss to avoid work and watch the next [your favorite team and sport] game. 
//The function takes as input the day and time of the game, which you provide manually.
async Task<string> GetExcuseEmail(string day_and_time)
{
    var excuse_email = await kernel.InvokeAsync<string>(prompts["excuses"],
    new() {
            { "day_and_time", day_and_time }
        }
    );

    Console.WriteLine(excuse_email);
    return excuse_email;
}


// CHALLENGE 2.2
// https://learn.microsoft.com/en-us/training/modules/guided-project-create-ai-travel-agent/1-introduction
async Task<string> ExecuteWithFunctions()
{
    var email = string.Empty;

    Console.WriteLine("What do you want to do? (e.g. 'write an excuse email')");
    var input = Console.ReadLine();

    var intent = await kernel.InvokeAsync<string>(
        prompts["get_intent"], 
        new() {{ "input",  input }}
    );
    Console.WriteLine(intent);

    switch (intent) {
        case "WriteEmail":
            Console.WriteLine("Who is your favourite football team?");
            var football_team = Console.ReadLine();

            var web_search_result = await SearchWebForFootballMatchDateAndTime(football_team);
            var day_and_time = await GetMatchDateAndTime(web_search_result);
            email = await GetExcuseEmail(day_and_time);
            break;
        default:
            Console.WriteLine("Sorry, I can't help with that.");
            break;
    }

    return email;
}


// CHALLENGE 2.2+
// Create an execution plan using Intelligent Planners
// https://learn.microsoft.com/en-us/training/modules/use-intelligent-planners/1-introduction
async Task<string> ExecuteWithIntelligentPlanners()
{
    var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions() { AllowLoops = true });

    string footballTeam = "Welling United";
    string goal = @$"The user's favourite football team is ${footballTeam}. 
        Create an excuse email for the user's boss so that they can be absent 
        from work on the date and time of the next scheduled ${footballTeam} match.";

    var plan = await planner.CreatePlanAsync(kernel, goal);
    Console.WriteLine(plan);

    var result = await plan.InvokeAsync(kernel);

    Console.WriteLine(result);
    return result;
}
#endregion
