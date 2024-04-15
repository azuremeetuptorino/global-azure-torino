using Azure;
using Azure.AI.OpenAI;
using Azure.AI.Translation.Text;
using ChatGptBot;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharpToken;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<List<FunctionsCall>>().Bind(builder.Configuration.GetSection($"{nameof(FunctionsCall)}s"));
var endpoint = builder.Configuration[$"{nameof(ChatGptSettings)}:azureChatGptEndPoint"];
ArgumentException.ThrowIfNullOrEmpty(endpoint);
var azureOpenAiApiKey = builder.Configuration[$"{nameof(ChatGptSettings)}:azureOpenAiApiKey"];
ArgumentException.ThrowIfNullOrEmpty(azureOpenAiApiKey);
var opt = new OpenAIClientOptions
{
    Diagnostics =
    {
        IsLoggingContentEnabled = true
    }
};
OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(azureOpenAiApiKey),opt);
builder.Services.AddSingleton(client);

builder.Services.RegisterByConvention<Program>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddOptions<ChatGptSettings>().Bind(builder.Configuration.GetSection(nameof(ChatGptSettings)));
builder.Services.AddOptions<Storage>().Bind(builder.Configuration.GetSection(nameof(Storage)));
builder.Services.AddOptions<PlaceholdersInformation>().Bind(builder.Configuration.GetSection(nameof(PlaceholdersInformation)));

var tikTokenEncoding = builder.Configuration[$"{nameof(ChatGptSettings)}:tikToken"];
var encoding = GptEncoding.GetEncoding(tikTokenEncoding);
builder.Services.AddSingleton(encoding);
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "localhost",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000/")
                            .AllowAnyHeader()
                            .AllowAnyOrigin()
                            .AllowAnyMethod();
                      });
});

var textTranslationKey = builder.Configuration[$"{nameof(ChatGptSettings)}:textTranslationKey"] ;
var textTranslationRegion= builder.Configuration[$"{nameof(ChatGptSettings)}:textTranslationRegion"]?? "westeurope";
ArgumentException.ThrowIfNullOrEmpty(textTranslationKey);
AzureKeyCredential credential = new(textTranslationKey);
TextTranslationClient textTranslationClient = new(credential, textTranslationRegion);
builder.Services.AddSingleton(textTranslationClient);


builder.Services.AddHttpClient();
builder.Services.TryAddSingleton<AzureEventSourceLogForwarder>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("localhost");

app.MapControllers();

app.Run();