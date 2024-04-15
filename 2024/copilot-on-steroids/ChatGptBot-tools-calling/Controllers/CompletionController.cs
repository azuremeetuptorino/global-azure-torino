using ChatGptBot.Dtos.Completion.Controllers;
using ChatGptBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatGptBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompletionController : ControllerBase
    {
        private readonly ICompletionService _completionService;


        public CompletionController(ICompletionService completionService)
        {
            _completionService = completionService;
        }

        [HttpPost("ask", Name = "Ask")]
        public async Task<AnswerToUserDto> Ask(UserQuestionDto userQuestion)
        {
            try
            {
                return await _completionService.Ask(userQuestion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;  
            }
         
        }

    }

       
    }
