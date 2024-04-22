using System.Net;

namespace ChatGptBot.Services.FunctionsCalling;

public class FunctionCallResult
{
    public string Result { get; init; }
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;    
}