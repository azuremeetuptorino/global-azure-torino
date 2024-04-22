namespace ChatGptBot.Dtos.Embeddings;

public class EmbedRequestBase
{
    public List<string> IgnoreIfStartsWith { get; init; } = new List<string> { "![" };
}