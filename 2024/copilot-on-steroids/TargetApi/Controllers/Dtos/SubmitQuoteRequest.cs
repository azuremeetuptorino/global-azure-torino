using Swashbuckle.AspNetCore.Annotations;

namespace FunctionCalling.Controllers.Dtos;

public record SubmitQuoteRequest : AvailableQuote
{
    [SwaggerSchema("email of the user", Nullable = false)]
    public string Email { get; init; } = "";

    [SwaggerSchema("cargo weight", Nullable = false)]
    public float? Weight { get; init; }

    [SwaggerSchema("cargo weight unit", Nullable = false)]
    public WeightUnit? WeightUnit { get; init; }

    [SwaggerSchema("commodity group", Nullable = false)]
    public string CommodityGroup { get; set; } = "";
}