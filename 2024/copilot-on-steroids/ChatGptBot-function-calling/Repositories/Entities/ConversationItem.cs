using Azure.AI.OpenAI;
using ChatGptBot.Chain.Dto;

namespace ChatGptBot.Repositories.Entities;

public record ConversationItem
{
    public required Guid ConversationId { get; init; }
    public Guid Id { get; init; } = Guid.NewGuid();
    

    
    public required string EnglishText { get; init; } = "";

    public required int Tokens { get; init; } 

    public required string ChatRole { get; init; }

    public DateTimeOffset At { get; init; } = DateTimeOffset.UtcNow;

    public bool? ProvideContext { get; init; } 

    public List<ContextMessage> ContextMessages { get; init; } = new();


}

public record ContextMessage 
{
    public required float Proximity { get; init; } = 0f;

    public required Guid RelatedConversationHistoryItem { get; init; } = Guid.Empty;
    public required Guid EmbeddingId { get; set; }
}
