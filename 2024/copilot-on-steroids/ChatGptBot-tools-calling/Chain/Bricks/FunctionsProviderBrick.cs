using ChatGptBot.Chain;
using ChatGptBot.Chain.Dto;
using ChatGptBot.Dtos.Completion.Controllers;
using ChatGptBot.Ioc;

using ChatGptBot.Services.FunctionsCalling;
using SharpToken;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatGptBot.LangChain.Bricks;

public class FunctionsProviderBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly IFunctionsProvider _functionsProvider;
    private readonly GptEncoding _gptEncoding;

    public FunctionsProviderBrick(IFunctionsProvider functionsProvider, GptEncoding gptEncoding)
    {
        _functionsProvider = functionsProvider;
        _gptEncoding = gptEncoding;
    }
    public override async Task<Answer> Ask(Question question)
    {
        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        foreach(var function in await _functionsProvider.AvailableFunctions())
        {
            question.Tools.Add((function, _gptEncoding.Encode(System.Text.Json.JsonSerializer.Serialize(function)).Count));
        }
        var ret = await Next.Ask(question);
        return ret;
    }
}