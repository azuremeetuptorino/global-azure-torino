using Azure.AI.OpenAI;
using ChatGptBot.Chain.Dto;
using ChatGptBot.Ioc;
using ChatGptBot.Services.FunctionsCalling;
using ChatGptBot.Settings;
using Microsoft.Extensions.Options;

namespace ChatGptBot.Chain.Bricks;

public class CompletionEndpointBrick : LangChainBrickBase, ILangChainBrick, ISingletonScope
{
    private readonly OpenAIClient _openAiClient;
    private readonly IFunctionCaller _functionCaller;

    public CompletionEndpointBrick(OpenAIClient openAiClient, IFunctionCaller functionCaller)
    {
        _openAiClient = openAiClient;
        _functionCaller = functionCaller;
    }

    public override async Task<Answer> Ask(Question question)
    {
        if (string.IsNullOrEmpty(question.UserQuestion.Text))
        {
            throw new Exception($"{nameof(Question)} is null");
        }

        var chatCompletionsOptions = new ChatCompletionsOptions();
        question.SystemMessages.ForEach(systemMessage =>
        {
            if (!string.IsNullOrEmpty(systemMessage.Text))
            {
                chatCompletionsOptions.Messages.Add(new
                    ChatRequestSystemMessage(systemMessage.Text));
            }
        });



        AddChatHistory(chatCompletionsOptions, question.ConversationHistoryMessages);

        chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage($"user prompt: {question.UserQuestion.Text}"));

        chatCompletionsOptions.Temperature = question.QuestionOptions.Temperature;
        foreach (var t in question.Tools.Select(f => f.Tool))
        {
            chatCompletionsOptions.Tools.Add(t);
        }
    

        chatCompletionsOptions.DeploymentName = question.ModelName;
        var response = await _openAiClient.GetChatCompletionsAsync(chatCompletionsOptions);
            var choice = response.Value.Choices[0];


            if (choice.FinishReason == CompletionsFinishReason.ToolCalls && choice.Message.ToolCalls.Count>0)
            {
                // should i add ChatRequestAssistantMessage and ChatRequestToolMessage to saved history ? 
                var toolCallHistoryMessage = new ChatRequestAssistantMessage(choice.Message);
                chatCompletionsOptions.Messages.Add(toolCallHistoryMessage);
                var functionCalls = choice.Message.ToolCalls.OfType<ChatCompletionsFunctionToolCall>();
                foreach (var functionCall in functionCalls)
                {
                    var ret = await _functionCaller.CallFunction(functionCall);
                    chatCompletionsOptions.Messages.Add(new ChatRequestToolMessage(ret.Result, functionCall.Id));
                }
                chatCompletionsOptions.ToolChoice = ChatCompletionsToolChoice.None;
                // i should check for token limit 
                response = await _openAiClient.GetChatCompletionsAsync(
                    chatCompletionsOptions);
                return new Answer { AnswerFromChatGpt= response.Value.Choices.ToList()[0].Message.Content ?? "" };
            }
            else
            {
                return new Answer { AnswerFromChatGpt = choice.Message.Content ?? "" };
            }

        
        
    }

    private static void AddChatHistory(ChatCompletionsOptions chatCompletionsOptions, List<ConversationHistoryMessage> conversationHistoryMessages)
    {

        foreach (var conversationHistoryMessage in conversationHistoryMessages)
        {
            if(ChatMessageExtensions.ChatRoleFromString(conversationHistoryMessage.ChatRole) == ChatRole.User)
            {
                chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(conversationHistoryMessage.Text));
            }
            else
            {
                chatCompletionsOptions.Messages.Add(new ChatRequestAssistantMessage(conversationHistoryMessage.Text));
            }
        }


    }

}

