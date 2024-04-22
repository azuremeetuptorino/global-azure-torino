namespace FunctionCalling.Validators.CustomValidators;

public class ValidationErrorInfo
{
    public string ToJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
    public string ErrorCode { get; init; } = "";
    public string AssistantAction { get; init; } = "";
}