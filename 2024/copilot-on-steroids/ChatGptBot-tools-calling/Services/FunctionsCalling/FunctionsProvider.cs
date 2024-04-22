using System.Text.Json.Nodes;
using Azure.AI.OpenAI;

using ChatGptBot.Ioc;
using ChatGptBot.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSwag;

namespace ChatGptBot.Services.FunctionsCalling
{
    public interface IFunctionsProvider
    {
        Task<IEnumerable<ChatCompletionsFunctionToolDefinition>> AvailableFunctions();

       
    }

    public class FunctionsProvider : IFunctionsProvider, ISingletonScope
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IEnumerable<FunctionsCall> _functionsCalls;

        public FunctionsProvider(IMemoryCache memoryCache, IOptions<List<FunctionsCall>> functionsCalls)
        {
            _memoryCache = memoryCache;
            _functionsCalls = functionsCalls.Value;
        }
        public async Task<IEnumerable<ChatCompletionsFunctionToolDefinition>> AvailableFunctions()
        {
            var ret = new List<ChatCompletionsFunctionToolDefinition>();
            foreach (var functionsCall in _functionsCalls)
            {
                var jsonUrl = functionsCall.OpenAiDocUrl;
                var doc = _memoryCache.Get<OpenApiDocument>(jsonUrl);
                if (doc == null)
                {
                    doc = await OpenApiDocument.FromUrlAsync(jsonUrl);
                    _memoryCache.Set(jsonUrl, doc, DateTime.MaxValue);
                }
                var operationDescription = 
                    doc.Operations.SingleOrDefault(o=> o.Operation.OperationId == functionsCall.Name);
                if(operationDescription!=null) 
                {
                    if (operationDescription.Operation.ActualParameters.Count == 1)
                    {
                        var chatCompletionFunctions = new ChatCompletionsFunctionToolDefinition
                        {
                            Parameters = new BinaryData(operationDescription.Operation.ActualParameters.ToList()[0]
                                .ActualTypeSchema.ToJson()),
                            Description = operationDescription.Operation.Description,
                            Name = operationDescription.Operation.OperationId,
                            
                        };

                        ret.Add(chatCompletionFunctions);
                    }
                }
            }

           
           
            return ret;
        }

      
    }

  
}

