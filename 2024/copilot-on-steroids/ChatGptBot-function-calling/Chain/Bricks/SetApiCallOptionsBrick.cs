using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Chain.Bricks;

public class SetApiCallOptionsBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly ChatGptSettings _chatGptSettings;

    public SetApiCallOptionsBrick(IOptions<ChatGptSettings> openAi)
    {
        _chatGptSettings = openAi.Value;
    }

    public override async Task<Answer> Ask(Question question)
    {
        question.QuestionOptions.Temperature = _chatGptSettings.Temperature;

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        return await Next.Ask(question);
    }

}