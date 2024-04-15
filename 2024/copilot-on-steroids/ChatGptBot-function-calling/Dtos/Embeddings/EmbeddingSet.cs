namespace ChatGptBot.Dtos.Embeddings;

public class EmbeddingSet
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Code { get; init; } = "default";
    public string Description { get; init; } = "default";
}