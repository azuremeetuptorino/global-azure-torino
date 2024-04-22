using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Chain.Bricks;

public class QuestionLengthGuardBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly ChatGptSettings _chatGptSettings;
  

    public QuestionLengthGuardBrick(IOptions<ChatGptSettings> openAiSettings)
    {
        _chatGptSettings = openAiSettings.Value;
    }

    public override async Task<Answer> Ask(Question question)
    {
        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        if (question.UserQuestion.Tokens > _chatGptSettings.MaxAllowedTokenRatioForUserQuestion)
        {
            throw new Exception($"Please write your question using less words");
        }
        return await Next.Ask(question);
    }

}