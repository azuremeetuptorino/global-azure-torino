using Azure.AI.OpenAI;
using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Repositories;
using ChatGptBot.Repositories.Entities;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;
using SharpToken;
using ContextMessage = ChatGptBot.Repositories.Entities.ContextMessage;
using ChatGptBot.Chain;
namespace ChatGptBot.Chain.Bricks;

public class ConversationManagerBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly IConversationRepository _conversationRepository;
    private readonly GptEncoding _gptEncoding;
    private readonly ChatGptSettings _chatGptSettings;
    

    public ConversationManagerBrick(IConversationRepository conversationRepository, GptEncoding gptEncoding,
        IOptions<ChatGptSettings> chatGptSettings)
    {
        _conversationRepository = conversationRepository;
        _gptEncoding = gptEncoding;
        _chatGptSettings = chatGptSettings.Value;
        
    }

    public override async Task<Answer> Ask(Question question)
    {

        if (Next == null)
        {
            throw new Exception($"{GetType().Name} cannot be the last item of the chain");
        }
      
      
            var questionConversationItem = new ConversationItem
            {
                EnglishText = question.UserQuestion.Text,
                ChatRole = ChatMessageExtensions.ChatRoleToString(ChatRole.User),
                ConversationId = question.ConversationId,
                Tokens = question.UserQuestion.Tokens
            };


            if (question.ConversationId != Guid.Empty)
            {
                var items = await _conversationRepository.LoadConversation(question.ConversationId);

                
                question.ConversationHistoryMessages.AddRange(items.Select((item,counter) => new ConversationHistoryMessage
                {
                    Id = item.Id, ChatRole = item.ChatRole,
                    Text = item.EnglishText, Tokens = item.Tokens,
                    ProvideContext = item.ProvideContext
                }));
            }

            var ret = await Next.Ask(question);
            if (question.ConversationId != Guid.Empty)
            {
                ret.ConversationId = question.ConversationId;
                if (question.ConversationId != Guid.Empty)
                {
                    //.RelatedConversationHistoryItem == Guid.Empty is pretty fragile  
                    await _conversationRepository.StoreUserConversationItem(questionConversationItem);
                    // the assistant conversation item
                    await _conversationRepository.StoreAssistantConversationItem(
                        new ConversationItem
                        {
                            ConversationId = question.ConversationId,
                            EnglishText = ret.AnswerFromChatGpt,
                            ChatRole = ChatMessageExtensions.ChatRoleToString(ChatRole.Assistant),
                            At = DateTimeOffset.UtcNow,
                            Tokens = _gptEncoding.Encode(ret.AnswerFromChatGpt).Count,
                            ProvideContext = null
                        });
                }
            }

            return ret;
      
       
    }

   
}