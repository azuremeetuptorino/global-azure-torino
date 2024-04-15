namespace ChatGptBot.Repositories.Entities
{
    public class EmbeddingForDb
    {
        public Guid SetId { get; init; } = Guid.Empty;
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Text { get; init; } = "";

        public IEnumerable<(float Value, int Index)> VectorValues { get; init; } = new List<(float, int)>();
        public int Tokens { get; set; }
    }

}
