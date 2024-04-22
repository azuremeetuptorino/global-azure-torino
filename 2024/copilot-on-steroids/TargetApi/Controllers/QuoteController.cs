using FluentValidation;
using FunctionCalling.Controllers.Dtos;
using FunctionCalling.ExternalServices.Mdm;
using FunctionCalling.Validators;
using FunctionCalling.Validators.CustomValidators;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json;
using FluentValidation.Results;
using FunctionCalling.ExternalServices.Mdm.Dto;
using FunctionCalling.Repository.Quotes;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace FunctionCalling.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuotesController : ControllerBase
    {

       

        private readonly ILogger<QuotesController> _logger;
        private readonly IValidator<GetQuotesByUserRequest> _getQuotesByUserRequestValidator;
        private readonly IValidator<QuotationQueryRequest> _quotationQueryRequestValidator;
        private readonly IValidator<SubmitQuoteRequest> _submitQuoteRequestValidator;
        private readonly IMdm _mdm;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ICommodityRepository _commodityRepository;

        public QuotesController(ILogger<QuotesController> logger,
            IValidator<GetQuotesByUserRequest> getQuotesByUserRequestValidator,  IValidator<QuotationQueryRequest> quotationQueryRequestValidator
            , IValidator<SubmitQuoteRequest> submitQuoteRequestValidator, IMdm mdm,IQuoteRepository quoteRepository,
            ICommodityRepository commodityRepository)
        {
            _logger = logger;
            _getQuotesByUserRequestValidator = getQuotesByUserRequestValidator;
            _quotationQueryRequestValidator = quotationQueryRequestValidator;
            _submitQuoteRequestValidator = submitQuoteRequestValidator;
            _mdm = mdm;
            _quoteRepository = quoteRepository;
            _commodityRepository = commodityRepository;
        }
        private readonly Random _random = new Random();

        [HttpPost(template: "submitted-quotes", Name = nameof(GetQuotesByUser))]
        [SwaggerOperation(
            Description = "returns the list of quotes submitted by the user",
            OperationId = nameof(GetQuotesByUser))]
        public Results<BadRequest<IEnumerable<ValidationErrorInfo>>, Ok<List<SubmittedQuote>>> GetQuotesByUser(
            [FromBody] GetQuotesByUserRequest getQuotesByUserRequest)
        {
            var result = _getQuotesByUserRequestValidator.Validate(getQuotesByUserRequest);
            if (result.Errors.Any())
            {
                return TypedResults.BadRequest(result.Errors.Select(e =>
                    JsonSerializer.Deserialize<ValidationErrorInfo>(e.ErrorMessage) ?? new ValidationErrorInfo()));
            }
            return TypedResults.Ok(_quoteRepository.GetUserQuotes(getQuotesByUserRequest));
        }


    

    [HttpPost(template: "available-quotes", Name = nameof(QuotationQueryRequest))]
        [SwaggerOperation(
            Description = "returns a list of available quotes according to the input values provided by the user",
            OperationId = nameof(QuotationQueryRequest))]
        public async Task<Results<BadRequest<IEnumerable<ValidationErrorInfo>>, Ok<List<AvailableQuote>>>> AvailableQuotes([FromBody] QuotationQueryRequest quotationQueryRequest)
        {
            quotationQueryRequest= quotationQueryRequest with { Origin = SanitizePortName(quotationQueryRequest.Origin), Destination = SanitizePortName(quotationQueryRequest.Destination) };
            
            var result = await _quotationQueryRequestValidator.ValidateAsync(quotationQueryRequest);
            if (result.Errors.SingleOrDefault(e => e.PropertyName == nameof(QuotationQueryRequest.Destination)) == null)
            {
                result.Errors.AddRange((ValidatePort(nameof(QuotationQueryRequest.Destination), quotationQueryRequest.Destination)).Errors);
            }
            if (result.Errors.SingleOrDefault(e => e.PropertyName == nameof(QuotationQueryRequest.Origin)) == null)
            {
                result.Errors.AddRange((ValidatePort(nameof(QuotationQueryRequest.Origin), quotationQueryRequest.Origin)).Errors);
            }
            if (result.Errors.SingleOrDefault(e => e.PropertyName == nameof(QuotationQueryRequest.CommodityGroup)) == null)
            {
                result.Errors.AddRange((ValidateCommodity(nameof(QuotationQueryRequest.CommodityGroup), quotationQueryRequest.CommodityGroup)).Errors);
            }

            if (result.Errors.Any())
            {
                return TypedResults.BadRequest(result.Errors.Select(e => JsonSerializer.Deserialize<ValidationErrorInfo>(e.ErrorMessage) ?? new ValidationErrorInfo()));
            }

            var quotes = new List<AvailableQuote>();
            Enumerable.Range(0, _random.Next(2, 4)).ToList().ForEach(i =>
            {
                quotes.Add(new AvailableQuote
                {
                    Amount = DemoHelpers.RandomNumber(500, 1456),
                    Currency = "USD",
                    ShippingWindowsFrom = DateTime.Now.AddDays(5),
                    ShippingWindowsTo = DateTime.Now.AddDays(35),
                    TransitDays = DemoHelpers.RandomNumber(13, 21),
                    // checked by validator
                    ContainerType = quotationQueryRequest.ContainerType.Value, // validator will check this
                    Origin = TryMatchPort(quotationQueryRequest.Origin).Single().PortVersion.Name,
                    Destination= TryMatchPort(quotationQueryRequest.Destination).Single().PortVersion.Name,
                });
            });
            return TypedResults.Ok(quotes); 
        }

       

       

        [HttpPost(template: "submit-quote", Name = nameof(SubmitQuoteRequest))]
        [SwaggerOperation(
            Description = "call this function to let the user submit a quote",
            OperationId = nameof(SubmitQuoteRequest))]
        public Results<BadRequest<IEnumerable<ValidationErrorInfo>>, Ok<SubmittedQuote>> SubmitQuote([FromBody] SubmitQuoteRequest submitQuoteRequest)
        {
            submitQuoteRequest = submitQuoteRequest  with { Origin = SanitizePortName(submitQuoteRequest.Origin), Destination = SanitizePortName(submitQuoteRequest.Destination) };
            var result = _submitQuoteRequestValidator.Validate(submitQuoteRequest);
            if (result.Errors.Any())
            {
                return TypedResults.BadRequest(result.Errors.Select(e => JsonSerializer.Deserialize<ValidationErrorInfo>(e.ErrorMessage) ?? new ValidationErrorInfo()));
            }

            if (result.Errors.SingleOrDefault(e => e.PropertyName == nameof(QuotationQueryRequest.Destination)) == null)
            {
                result.Errors.AddRange((ValidatePort(nameof(QuotationQueryRequest.Destination), submitQuoteRequest.Destination)).Errors);
            }
            if (result.Errors.SingleOrDefault(e => e.PropertyName == nameof(QuotationQueryRequest.Origin)) == null)
            {
                result.Errors.AddRange((ValidatePort(nameof(QuotationQueryRequest.Origin), submitQuoteRequest.Origin)).Errors);
            }
            if (result.Errors.SingleOrDefault(e => e.PropertyName == nameof(QuotationQueryRequest.CommodityGroup)) == null)
            {
                result.Errors.AddRange((ValidateCommodity(nameof(QuotationQueryRequest.CommodityGroup), submitQuoteRequest.CommodityGroup)).Errors);
            }

            if (result.Errors.Any())
            {
                return TypedResults.BadRequest(result.Errors.Select(e => JsonSerializer.Deserialize<ValidationErrorInfo>(e.ErrorMessage) ?? new ValidationErrorInfo()));
            }

            var quote = new SubmittedQuote
            {
                CommodityGroup = submitQuoteRequest.CommodityGroup,
                Weight = submitQuoteRequest.Weight.Value,
                WeightUnit = submitQuoteRequest.WeightUnit.Value,
                QuoteNumber = $"Q{DemoHelpers.RandomString(5)}",
                Id = _random.Next(0, int.MaxValue),
                Status = QuoteStatus.Submitted,
                CreationDate = DateTimeOffset.Now,
                Email = submitQuoteRequest.Email,
                ExpirationDate = DateTimeOffset.Now.AddDays(_random.Next(10, 50)),
                Amount = submitQuoteRequest.Amount,
                ContainerType = submitQuoteRequest.ContainerType,
                Currency = submitQuoteRequest.Currency,
                ShippingWindowsFrom = submitQuoteRequest.ShippingWindowsFrom,   
                ShippingWindowsTo = submitQuoteRequest.ShippingWindowsTo,   
                TransitDays=submitQuoteRequest.TransitDays ,
                Origin = submitQuoteRequest.Origin,
                Destination = submitQuoteRequest.Destination,
                

            };
            _quoteRepository.Add(quote);
            return TypedResults.Ok(quote);
        }

        private ValidationResult ValidateCommodity(string propertyName, string propertyValue)
        {
            var ret = new ValidationResult();
            if (!string.IsNullOrEmpty(propertyValue))
            {
                var i = propertyValue.IndexOf("with code:",StringComparison.OrdinalIgnoreCase);
                if (i != -1)
                {
                    propertyValue = propertyValue.Substring(i+ "with code:".Length);
                }

                var candidateCommodity = TryMatchCommodity(propertyValue);

                if (candidateCommodity.Count == 0)
                {

                    ret.Errors.Add(new ValidationFailure(propertyName,
                        (new ValidationErrorInfo
                        {
                            ErrorCode = $"invalid value provided for {propertyName}",
                            AssistantAction =
                                $"reply to the user with these exact words: '{propertyValue} is an invalid value for {propertyName}'"
                        }.ToJson())));
                }
                else if (candidateCommodity.Count > 1)
                {
                    ret.Errors.Add(new ValidationFailure(propertyName,
                        (new ValidationErrorInfo
                        {
                            ErrorCode = $"ambiguous value provided for {propertyName}",
                            AssistantAction =
                                $"reply to the user with these exact words: \"choose among the following values for {propertyName} : {string.Join(',', candidateCommodity.Take(5).Select(FormatCommodity))}\""
                        }.ToJson())));
                }
            }


            return ret;
        }

        private string SanitizePortName(string port)
        {
            if (!string.IsNullOrEmpty(port))
            {
                var unCode = Regex.Match(port, @"(?<=\[).*?(?=\])").Value;
                if (!string.IsNullOrEmpty(unCode))
                {
                    port = unCode;
                }
            }
            return port??"";
        }

        private ValidationResult ValidatePort(string propertyName,string portName)
        {
            var ret = new ValidationResult();
            if (!string.IsNullOrEmpty(portName))
            {
                var candidatePorts = TryMatchPort(portName);

                if (candidatePorts.Count == 0)
                {

                    ret.Errors.Add(new ValidationFailure(propertyName,
                        (new ValidationErrorInfo
                        {
                            ErrorCode = $"invalid value for {propertyName}",
                            AssistantAction =
                                $"reply to the user with these exact words: '{portName} is an invalid value for {propertyName}'"
                        }.ToJson())));
                }
                else if (candidatePorts.Count > 1)
                {
                    ret.Errors.Add(new ValidationFailure(propertyName,
                        (new ValidationErrorInfo
                        {
                            ErrorCode = $"ambiguous value for {propertyName}",
                            AssistantAction =
                                $"reply to the user with these exact words: \"choose among the following values for {propertyName} : {string.Join(',',candidatePorts.Take(5).Select(FormatPort))}\""
                        }.ToJson())));
                }
            }


            return ret;
        }

        private IReadOnlyCollection<PortDetails> TryMatchPort(string propertyValue)
        {
            var candidatePorts = (_mdm.GetPortByUnCodeOrName(propertyValue));
            var searchMatchByUnCode = candidatePorts
                .Where(l => l.LongDisplayName.Contains($"[{propertyValue}]", StringComparison.OrdinalIgnoreCase)).ToList();
            if (searchMatchByUnCode.Count > 0)
            {
                candidatePorts = searchMatchByUnCode;
            }

            return candidatePorts;
        }

        private IReadOnlyCollection<Commodity> TryMatchCommodity(string propertyValue)
        {
            return  _commodityRepository.GetCommodityByDescriptionOrCode(propertyValue);
        }

        private string FormatPort(PortDetails port)
        {
            return $"{port.LongDisplayName}";
        }

        private string FormatCommodity(Commodity commodity)
        {
            return $"{commodity.Description} with code: {commodity.Code}";
        }




    }

   
}