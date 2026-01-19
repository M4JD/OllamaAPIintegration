//using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
var ollamaEndpoint = builder.Configuration["AI__Ollama__Endpoint"] ?? "http://localhost:11434";
var chatModel = builder.Configuration["AI__Ollama__ChatModel"] ?? "llama3.2";

// Create and configure Ollama client
var uri = new Uri("http://ollama:11434");
//IChatClient client
var ollama = new OllamaApiClient(uri);

//ollama.ChatAsync();
var chat = new Chat(ollama, chatModel);
chat.SendAsync("", default);

var models = await ollama.ListLocalModelsAsync();
var runningModels = await ollama.ListRunningModelsAsync();
foreach (var model in runningModels)
    Console.WriteLine($"Name: {model.Name}, Size: {model.Size}, Details: {model.Details}");
foreach (var model in models)
    Console.WriteLine($"Name: {model.Name}, Size: {model.Size}, Details: {model.Details}");

//await foreach (var stream in ollama.GenerateAsync(new OllamaSharp.Models.GenerateRequest
//{
//    Model = chatModel,
//    Prompt = "can you help me code?"
//}))

    //Console.Write(stream?.Response);

var res = ollama.ChatAsync(new OllamaSharp.Models.Chat.ChatRequest
{
    Model = chatModel,
    Messages =
    [
        new("user", "How is the weather today?")
    ]
});
string messageResponse = string.Empty;
await foreach (var item in res)
{
    Console.Write($"{item?.Message?.Content} , {item?.Done}");
    messageResponse += $"{item?.Message?.Content}";
}
Console.Write($"response message: {messageResponse}");
//add if not already available locally
//await foreach (var status in ollama.PullModelAsync("llama3.2"))
//    Console.WriteLine($"{status.Percent}% {status.Status}");

//IChatClient client = new OllamaChatClient(ollamaEndpoint, modelId: chatModel)
//    .AsBuilder()
//    .UseFunctionInvocation()
//    .Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
