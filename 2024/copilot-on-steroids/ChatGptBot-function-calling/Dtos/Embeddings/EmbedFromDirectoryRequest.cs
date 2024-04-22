namespace ChatGptBot.Dtos.Embeddings;

public class EmbedFromDirectoryRequest : EmbedRequestBase
{
    public Guid SetId { get; init; } = Guid.Empty;
    public string DirectoryPath { get; init; } = "";
    public string SearchPattern { get; init; } = "*.md";

    public bool Recursive { get; init; } = true;

}