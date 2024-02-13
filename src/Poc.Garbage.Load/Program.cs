using NBomber.CSharp;
using NBomber.Http.CSharp;

string workstationApi = @"https://pocgarbageworkstationapi20240212094046.azurewebsites.net/";
string serverApi = @"https://pocgarbageserverapi20240212085220.azurewebsites.net/";

using var httpClient = new HttpClient();

var wsScenario = Scenario.Create("workstation_gc_scenario", async context =>
{
    var request =
        Http.CreateRequest("GET", $"{workstationApi}gc");
    var response = await Http.Send(httpClient, request);

    return response;
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.Inject(rate: 3,
                      interval: TimeSpan.FromSeconds(1),
                      during: TimeSpan.FromMinutes(10))
);

var serverScenario = Scenario.Create("server_gc_scenario", async context =>
{
    var request =
    Http.CreateRequest("GET", $"{serverApi}gc");
        //.WithHeader("Accept", "text/html");
    // .WithHeader("Accept", "application/json")
    // .WithBody(new StringContent("{ id: 1 }", Encoding.UTF8, "application/json");
    // .WithBody(new ByteArrayContent(new [] {1,2,3}))


    var response = await Http.Send(httpClient, request);

    return response;
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.Inject(rate: 3,
                      interval: TimeSpan.FromSeconds(1),
                      during: TimeSpan.FromMinutes(10))
);

NBomberRunner
    .RegisterScenarios(wsScenario, serverScenario)
    .Run();