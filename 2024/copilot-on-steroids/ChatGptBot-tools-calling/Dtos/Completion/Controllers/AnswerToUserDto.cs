namespace ChatGptBot.Dtos.Completion.Controllers;

public class AnswerToUserDto
{
    public string Answer { get; set; } = "";
    public Guid ConversationId { get; set; } = Guid.Empty;

    public string QuestionLanguageCode { get; set; } = "";
}