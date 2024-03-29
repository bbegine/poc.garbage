﻿using System.Net.Http.Json;

using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

using Poc.Garbage.Load;

// Make sure this is the same value as the ENV:poc_gc_app_prefix variable in the setVariables.ps1 script.
var appPrefix = "app-poc-gc";

// The interval represents a time when [rate] amount of requests will be send to ech app
var interval = TimeSpan.FromSeconds(1);
// The number of requests that will be send to the app's per interval
var minRate = 5;
var maxRate = 30;
// The total amount of time the app's will be under load
var during = TimeSpan.FromMinutes(10);
// The string size that will be created in the api's
var stringSizeInKB = 2;
// The number of strings that will be created in the api
var amountOfStrings = 3000;

using var httpClient = new HttpClient();

List<ScenarioProps> scenarios = new();
for (int i = 0; i < 6; i++)
{
    var scName = $"poc_gc_{i + 1}_scenario";
    var url = $"https://{appPrefix}-{i + 1}.azurewebsites.net/consumers";

    var wsScenario = Scenario.Create(scName, async context =>
    {
        var request = Http
            .CreateRequest(
                "POST",
                url)
            .WithHeader("Accept", "application/json")
            .WithBody(
                JsonContent.Create(
                    new ConsumerDto(stringSizeInKB, amountOfStrings)
                ));
        var response = await Http.Send(httpClient, request);

        return response;
    })
    .WithoutWarmUp()
    .WithLoadSimulations(
        Simulation.InjectRandom(
            minRate: minRate,
            maxRate: maxRate,
            interval,
            during)
    );

    scenarios.Add(wsScenario);
}

var result = NBomberRunner
    .RegisterScenarios(scenarios.ToArray())
    .Run();
