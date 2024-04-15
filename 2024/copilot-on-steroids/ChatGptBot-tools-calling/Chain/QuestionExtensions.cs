using ChatGptBot.Chain.Bricks;
using ChatGptBot.Chain.Dto;

namespace ChatGptBot.Chain;

public static class QuestionExtensions
{
    

    

    public static void RemoveOldestConversationEntryPair(this Question question)
    {
        if (question.ConversationHistoryMessages.Count > 1)
        {
            question.ConversationHistoryMessages.RemoveAt(0);
            question.ConversationHistoryMessages.RemoveAt(0);
        }
    }

    
}