namespace ChatGptBot.Dtos.Completion.Controllers;

public class UserQuestionDto
{
    public Guid ConversationId { get; init; } = Guid.NewGuid();
    public string QuestionText { get; init; } = "";
}