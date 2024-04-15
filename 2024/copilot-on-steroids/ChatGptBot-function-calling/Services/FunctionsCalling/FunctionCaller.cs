using System.Text;
using Azure.AI.OpenAI;

using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Services.FunctionsCalling;

public interface IFunctionCaller
{
       
    Task<FunctionCallResult> CallFunction(FunctionCall functionCall);
}

public class FunctionCaller : IFunctionCaller, ISingletonScope
{
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IEnumerable<FunctionsCall> _functionsCalls;

    public FunctionCaller(IHttpClientFactory httpClientFactory, IOptions<List<FunctionsCall>> functionsCalls)
    {
        _httpClientFactory = httpClientFactory;

        _functionsCalls = functionsCalls.Value;
    }

    public async Task<FunctionCallResult> CallFunction(FunctionCall functionCall)
    {
       
        var functionsCall = _functionsCalls.SingleOrDefault(f => f.Name == functionCall.Name);
        ArgumentNullException.ThrowIfNull(functionsCall);
        var client = _httpClientFactory.CreateClient();
        HttpContent json = new StringContent(functionCall.Arguments, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(functionsCall.Url, json);
        return new FunctionCallResult {StatusCode = response.StatusCode, Result = await response.Content.ReadAsStringAsync()};
    }
}