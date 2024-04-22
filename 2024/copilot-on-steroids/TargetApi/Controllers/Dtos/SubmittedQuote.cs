using System.Collections.Generic;

namespace FunctionCalling.Controllers.Dtos;

public class SubmittedQuote
{
    public int Id { get; init; }

    public string QuoteNumber { get; init; } = "";

    public string Email { get; init; } = "";

    public DateTimeOffset ExpirationDate { get; init; }

    public QuoteStatus Status { get; init; }

    public DateTimeOffset CreationDate { get; init; }

    public float Amount { get; init; }
    public string Currency { get; init; } = "";

    public int TransitDays { get; init; }

    public DateTime ShippingWindowsFrom { get; init; }

    public DateTime ShippingWindowsTo { get; init; }

    public ContainerType ContainerType { get; init; }
    public required string Origin { get; init; } = "";
    public required  string Destination { get; init; } = "";

    public required float Weight { get; init; }
    public required WeightUnit WeightUnit { get; init; }

    public required string CommodityGroup { get; init; }
}

public enum QuoteStatus
{
    Submitted, Expired
}
//public enum QuoteStatusQuery
//{
//    Submitted, Expired
//}

