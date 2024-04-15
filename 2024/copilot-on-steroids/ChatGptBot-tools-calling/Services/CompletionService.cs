using AutoMapper;
using ChatGptBot.Chain;
using ChatGptBot.Chain.Bricks;
using ChatGptBot.Chain.Dto;
using ChatGptBot.Dtos.Completion.Controllers;
using ChatGptBot.Ioc;
using ChatGptBot.LangChain.Bricks;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;
using SharpToken;

namespace ChatGptBot.Services
{
    public interface ICompletionService
    {
        Task<AnswerToUserDto> Ask(UserQuestionDto question);
    }

    public class CompletionService : ICompletionService, ISingletonScope
    {
        private readonly GptEncoding _gptEncoding;
        private readonly IMapper _mapper;
        private readonly ChatGptSettings _chatGptSettings;
        private readonly ILangChainBrick _chain;
        
        public CompletionService(ILangChainBuilderFactory langChainBuilderFactory, 
            IEnumerable<ILangChainBrick> langChainBricks, GptEncoding gptEncoding,IMapper mapper, IOptions<ChatGptSettings> chatGptSettings)
        {
            _gptEncoding = gptEncoding;
            _mapper = mapper;
            _chatGptSettings = chatGptSettings.Value;
            IEnumerable<ILangChainBrick> langChainBricks1 = langChainBricks.ToList();
            var builder = langChainBuilderFactory.Create();
            var chainBricks = langChainBricks1.ToList();
            // NAKED S 
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));
            //  E 

            // 1) history and temperature
            //builder.Add(chainBricks.
            //    Single(i => i.GetType() == typeof(SetSystemMessageBrick)));
            //builder.Add(chainBricks.
            //    Single(i => i.GetType() == typeof(ConversationManagerBrick)));
            //builder.Add(chainBricks.
            //    Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            //builder.Add(chainBricks.
            //    Single(i => i.GetType() == typeof(CompletionEndpointBrick)));
            // E



            //// 2) history and temperature and context and system message
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(ConversationManagerBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetContextBrick)));
            //// OR ADD builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetContextBrickWithSql)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetSystemMessageBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));
            //////  E


            //// 3) above + translate S 
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(QuestionTranslatorBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(ConversationManagerBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(AnswerTranslatorBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetContextBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetSystemMessageBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));
            //// E

            //// 4) above + question user length guard, change topic detector 
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(QuestionTranslatorBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(ConversationManagerBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(AnswerTranslatorBrick)));
            //ADD builder.Add(chainBricks.Single(i => i.GetType() == typeof(ChangeTopicDetectorBrick))); 
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(AnswerTranslatorBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetSystemMessageBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetContextBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));
            ////  E

            // 5) above plus max token guard (user question and MaxTokenGuardBrick)
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(QuestionLengthGuardBrick)));
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(ConversationManagerBrick)));
            //builder.Add(chainBricks.Single(i => i.GetType() == typeof(ChangeTopicDetectorBrick)));
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetSystemMessageBrick)));
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(FunctionsProviderBrick))); 
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(MaxTokenGuardBrick)));
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(SetApiCallOptionsBrick)));
            builder.Add(chainBricks.Single(i => i.GetType() == typeof(CompletionEndpointBrick)));
            ////  E



            _chain = builder.BuildChain();
        }


        public async Task<AnswerToUserDto> Ask(UserQuestionDto userQuestion)
        {
            
            var question = new Question
            {
                ModelName = _chatGptSettings.ModelName,
                ConversationId = userQuestion.ConversationId,
                UserQuestion = new UserQuestionMessage
                {
                    
                    Text = userQuestion.QuestionText, 
                    Tokens = _gptEncoding.Encode(userQuestion.QuestionText).Count
                }
            };
            var ret = await _chain.Ask(question);
            return _mapper.Map<AnswerToUserDto>(ret);
        }
    }
}
