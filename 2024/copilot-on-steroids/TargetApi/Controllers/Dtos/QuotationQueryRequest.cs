using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Macross.Json.Extensions ;
using System.Text.Json;

namespace FunctionCalling.Controllers.Dtos;

[SwaggerSchema(Required = new[]
{
    nameof(Email), nameof(Origin),nameof(Destination), nameof(ContainerType)
    ,  nameof(Origin) ,  nameof(Weight),  nameof(CommodityGroup)
})]
public record QuotationQueryRequest
{
    [SwaggerSchema("The email of the user. IMPORTANT : Assistant must ask the user a value for email if not provided in the conversation. Assistant must not not make up one", Nullable = false)]
    public string Email { get; init; } = "";

    [SwaggerSchema("origin (from)", Nullable = false)]
    public string Origin { get; init; } = "";

    [SwaggerSchema("destination (to)", Nullable = false)]
    public string Destination { get; init; } = "";

    [SwaggerSchema("container type", Nullable = false)]
    public ContainerType? ContainerType { get; init; }

    [SwaggerSchema("commodity group", Nullable = false)]
    public string CommodityGroup { get; init; } = "";


    [SwaggerSchema("cargo weight", Nullable = false)]
    public float? Weight { get; init; }

    [SwaggerSchema("cargo weight unit", Nullable = false)]
    public WeightUnit? WeightUnit { get; init; }


}

public enum WeightUnit
{
    Kilograms,
    Pounds
}


public enum ContainerType
{
    [JsonPropertyName("20DV")]
    DV20,
    [JsonPropertyName("40DV")]
    DV40
}






