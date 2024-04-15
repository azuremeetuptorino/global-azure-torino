using Azure.AI.OpenAI;
using ChatGptBot.Dtos.Completion.Controllers;

namespace ChatGptBot.Chain.Dto;

public class Question
{
    public bool  UserHasChangedTopic { get; set; }=false;
    public string? EmbeddingSetCode { get; init; }
    public Guid ConversationId { get; set; } = Guid.NewGuid(); 
    public List<TextWIthTokenCount> SystemMessages { get; init; } = new();

    
   
    

    public List<ConversationHistoryMessage> ConversationHistoryMessages { get; init; } = new();

    public UserQuestionMessage UserQuestion { get; set; } = new() { Text = "", Tokens = 0 };

    public QuestionOptions QuestionOptions { get; } = new();
    public int? EmbeddingMatchMaxItems { get; set; }
    public required string ModelName { get; set; } = "";

    public List<(ChatCompletionsToolDefinition Tool, int Tokens)> Tools { get; init; } = new();
}
public record ContextMessage  : TextWIthTokenCount
{
    public required float Proximity { get; init; } = 0f;
    public required int QuestionSequenceNumberInConversation { get; init; }

    public required Guid ConversationItemId { get; init; } = Guid.Empty;
    public required Guid EmbeddingId { get; set; }
}
public record UserQuestionMessage : TextWIthTokenCount
{
    
    
}




public record ConversationHistoryMessage : TextWIthTokenCount
{
    public required string ChatRole { get; init; }
    public required Guid Id { get; init; }
    public bool? ProvideContext { get; set; }

    
}