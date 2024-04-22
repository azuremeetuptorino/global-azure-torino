namespace ChatGptBot.Chain.Dto;

public record TextWIthTokenCount
{
    public required string Text { get; set; }
    public required int Tokens { get; init; }
}