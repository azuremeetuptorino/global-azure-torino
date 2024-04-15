namespace ChatGptBot.Dtos.Embeddings;

public class EmbedFromFileRequest : EmbedRequestBase
{
    public Guid SetId { get; init; } = Guid.Empty;
    public string FilePath { get; init; } = "";
}