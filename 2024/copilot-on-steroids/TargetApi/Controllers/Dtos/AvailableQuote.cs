namespace FunctionCalling.Controllers.Dtos;

public record AvailableQuote
{
    public float Amount { get; init; } 
    public string Currency { get; init; } = "";

    public int TransitDays { get; init; }

    public DateTime ShippingWindowsFrom { get; init; }

    public DateTime ShippingWindowsTo { get; init; }

    public ContainerType ContainerType { get; init; }

    public string Origin { get; init; } = "";

    public string Destination{ get; init; } = "";


}