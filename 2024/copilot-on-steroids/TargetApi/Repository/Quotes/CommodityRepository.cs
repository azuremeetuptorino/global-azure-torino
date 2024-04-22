using FunctionCalling.Ioc;

namespace FunctionCalling.Repository.Quotes;

public class CommodityRepository : ICommodityRepository, ISingletonScope
{
    private List<Commodity> _commodities=new();   
    public CommodityRepository()
    {
        _commodities.AddRange(
            System.Text.Json.JsonSerializer.Deserialize<List<Commodity>>(
                File.ReadAllText("Repository\\Commodity.json")));
    }

    public List<Commodity> GetCommodityByDescriptionOrCode(string description)
    {
        return _commodities.Where(c=>c.Description.Contains(description,StringComparison.OrdinalIgnoreCase) || c.Code.Contains(description, StringComparison.OrdinalIgnoreCase)).ToList(); 
    }

    public List<Commodity> GetAll()
    {
        return _commodities;
    }
}

public interface ICommodityRepository
{
    List<Commodity> GetCommodityByDescriptionOrCode(string description);
    List<Commodity> GetAll();
}

public class Commodity
{
    public string Code { get; init; }="";
    public string Description { get; init; } = "";
    
}
