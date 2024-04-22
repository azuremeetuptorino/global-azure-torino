

using Azure.AI.OpenAI;
using ChatGptBot.Chain.Dto;

namespace ChatGptBot.Chain;

public static class ChatMessageExtensions
{
    public static int TotalTokensInQuestion(this Question question)
    {
        var totalInputTokens = 0;
        totalInputTokens = question.SystemMessages.Aggregate(totalInputTokens, (counter, item) => counter + item.Tokens);
        totalInputTokens = question.ConversationHistoryMessages.Aggregate(totalInputTokens, (counter, item) => counter + item.Tokens);
        totalInputTokens += question.UserQuestion.Tokens;
        return totalInputTokens;

    }

    public static string ChatRoleToString(ChatRole chatRole)
    {
        if (chatRole == ChatRole.Assistant) return "A";
        if (chatRole == ChatRole.System) return "S";
        if (chatRole == ChatRole.User) return "U";
        throw new Exception($"Unknown value for chat role {chatRole}");

    }
    public static ChatRole ChatRoleFromString(string chatRole)
    {
        if (chatRole == "A") return ChatRole.Assistant;
        if (chatRole == "S") return ChatRole.System;
        if (chatRole == "U") return ChatRole.User;
        throw new Exception($"Unknown value for chat role {chatRole}");

    }

}