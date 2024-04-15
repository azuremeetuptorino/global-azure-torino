using FunctionCalling.Controllers.Dtos;
using FunctionCalling.ExternalServices.Mdm;
using FunctionCalling.Ioc;
using static FunctionCalling.Repository.Quotes.QuoteRepository;

namespace FunctionCalling.Repository.Quotes
{
    public interface IQuoteRepository
    {
        List<SubmittedQuote> GetUserQuotes(GetQuotesByUserRequest getQuotesByUserRequest);
        void Add(SubmittedQuote quote);
    }
    public class QuoteRepository : IQuoteRepository,ISingletonScope
    {
       
        private readonly List<SubmittedQuote> _quotes= new List<SubmittedQuote>();
  
   
        public QuoteRepository(IMdm mdm,ICommodityRepository commodityRepository)
        {
            
           
            var ports = mdm.GetAll().ToList();
            var commodities = commodityRepository.GetAll().ToList();
            Enumerable.Range(0, DemoHelpers.RandomNumber(2, 3)).ToList().ForEach(i =>
            {
                _quotes.Add(new SubmittedQuote
                {
                    CommodityGroup = commodities[DemoHelpers.RandomNumber(0, commodities.Count)].Description,
                    Email = DemoHelpers.RandomEmail(),
                    Weight = DemoHelpers.RandomNumber(200,2000),
                    WeightUnit = DemoHelpers.RandomWeightUnit(),
                    Amount = DemoHelpers.RandomNumber(500, 1456),
                    Currency = "USD",
                    CreationDate = DateTime.Now.AddDays(-DemoHelpers.RandomNumber(1, 15)),
                    ExpirationDate= DateTime.Now.AddDays(DemoHelpers.RandomNumber(1, 15)),
                    QuoteNumber = DemoHelpers.RandomString(5),
                    ShippingWindowsFrom = DateTime.Now.AddDays(5),
                    ShippingWindowsTo = DateTime.Now.AddDays(35),
                    TransitDays = DemoHelpers.RandomNumber(13, 21),
                    ContainerType = DemoHelpers.RandomContainerType(),
                    Destination = ports[DemoHelpers.RandomNumber(0,ports.Count)].PortVersion.Name,
                    Origin = ports[DemoHelpers.RandomNumber(0, ports.Count)].PortVersion.Name,
                    Status = DemoHelpers.RandomQuoteStatus()
                });
            });
        }
       

        
     
       
        public List<SubmittedQuote> GetUserQuotes(GetQuotesByUserRequest getQuotesByUserRequest)
        {
            var quotes = _quotes.Where(b=> b.Email == getQuotesByUserRequest.Email 
                                           && (getQuotesByUserRequest.QuoteStatus == null ||  b.Status == getQuotesByUserRequest.QuoteStatus.Value)
                                           && (getQuotesByUserRequest.DateFrom== null || b.CreationDate >= getQuotesByUserRequest.DateFrom.Value)
                                           && (getQuotesByUserRequest.DateTo== null || b.CreationDate <= getQuotesByUserRequest.DateTo.Value)
                                           ).ToList();   
            return quotes;
        }

        public void Add(SubmittedQuote quote)
        {
            _quotes.Add(quote);
        }
       
    }
}
