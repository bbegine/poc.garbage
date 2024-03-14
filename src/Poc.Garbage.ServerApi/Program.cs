using System.Runtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app
    .MapPost("/consumers", async (ConsumerDto request) =>
    {
        var data = Enumerable
            .Range(0, request.NumberOfString)
            .Select(x => new string('D', request.StringSizeInKB * 1024)).ToArray();
        await Task.Delay(TimeSpan.FromMilliseconds(50));
        return Results.Ok(
            new ConsumerResponse(
                GC.GetGCMemoryInfo().FragmentedBytes,
                GCSettings.IsServerGC ? "Server" : "Workstation"));
    })
    .WithName("Consumers")
    .WithOpenApi();

app.MapGet("/_health", () =>
{
    return Results.Ok();
})
.WithName("Health")
.WithOpenApi();

app.Run();

record ConsumerDto(int StringSizeInKB, int NumberOfString);
record ConsumerResponse(long FragmentedBytes, string GarbageCollection);
