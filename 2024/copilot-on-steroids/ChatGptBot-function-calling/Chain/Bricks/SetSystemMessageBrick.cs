using System;
using System.IO;
using System.Threading.Tasks;
using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;
using SharpToken;

namespace ChatGptBot.Chain.Bricks;

public class SetSystemMessageBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly GptEncoding _gptEncoding;

    private readonly ChatGptSettings _chatGptSettings;

    public SetSystemMessageBrick(IOptions<ChatGptSettings> openAi, GptEncoding gptEncoding)
    {
        _gptEncoding = gptEncoding;
        _chatGptSettings = openAi.Value;
    }

    public override async Task<Answer> Ask(Question question)
    {
        var systemMessage = File.ReadAllText($"Settings{Path.DirectorySeparatorChar}systemMessage-{_chatGptSettings.SystemMessageName}.txt");
        if (!string.IsNullOrEmpty(systemMessage))
        {
            systemMessage = systemMessage.Replace("{today}", DateTime.Now.ToString("yyyy-MM-dd"));
            question.SystemMessages.Add(new TextWIthTokenCount
                {Text = systemMessage, Tokens = _gptEncoding.Encode(systemMessage).Count});
        }

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
        return await Next.Ask(question);
    }

}