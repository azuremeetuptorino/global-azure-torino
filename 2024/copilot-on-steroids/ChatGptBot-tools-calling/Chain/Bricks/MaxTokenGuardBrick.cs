using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Chain.Bricks;

public class MaxTokenGuardBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
   
    private readonly ChatGptSettings _chatGptSettings;


    public MaxTokenGuardBrick(IOptions<ChatGptSettings> openAi)
    {
      
        _chatGptSettings = openAi.Value;
    }

    public override async Task<Answer> Ask(Question question)
    {
        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }

        var totalTokensInQuestion = question.TotalTokensInQuestion();
        while (true)
        {
            // there are 50 tokens left for the answer 
            // chatpgpt says he is capable to adjust himself to adapt answer size to token max token limit
            if (totalTokensInQuestion + _chatGptSettings.MinimumAvailableTokensForTokenForAnswer <= _chatGptSettings.MaxTokens)
            {
                break;
            }

            question.RemoveOldestConversationEntryPair();   

            totalTokensInQuestion = question.TotalTokensInQuestion();
        }
        return await Next.Ask(question);
    }
}