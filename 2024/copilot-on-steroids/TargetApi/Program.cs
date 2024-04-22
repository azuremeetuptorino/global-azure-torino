using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using FluentValidation;
using FunctionCalling;
using FunctionCalling.ExternalServices;
using FunctionCalling.ExternalServices.Mdm;
using FunctionCalling.Ioc;
using Microsoft.Identity.Client;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Host
    .UseSerilog((context, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonPropertyNameStringEnumConverter())); 
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumMemberConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.UseInlineDefinitionsForEnums();
    
    c.SchemaFilter<EnumSchemaFilter>();
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.RegisterByConvention<Program>();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

