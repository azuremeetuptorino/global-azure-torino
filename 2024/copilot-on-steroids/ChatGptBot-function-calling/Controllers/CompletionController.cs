using ChatGptBot.Dtos.Completion.Controllers;
using ChatGptBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;

namespace ChatGptBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompletionController : ControllerBase
    {
        private readonly ICompletionService _completionService;


        public CompletionController(ICompletionService completionService, AzureEventSourceLogForwarder logForwarder)
        {
            logForwarder.Start();
            _completionService = completionService;
        }

        [HttpPost("ask",Name = "Ask")]
        public async Task<AnswerToUserDto> Ask(UserQuestionDto userQuestion)
        {
            
            return await _completionService.Ask(userQuestion);  

        }

       
    }
}