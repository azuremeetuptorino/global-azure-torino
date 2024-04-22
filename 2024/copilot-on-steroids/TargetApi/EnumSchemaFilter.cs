using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FunctionCalling;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            model.Enum.Clear();
            foreach (string enumName in EnumSchemaFilterExtensions.EnumGetNamesEx(context.Type))
            {
                System.Reflection.MemberInfo memberInfo = context.Type.GetMember(enumName).FirstOrDefault(m => m.DeclaringType == context.Type);
                EnumMemberAttribute enumMemberAttribute = memberInfo == null ? null : memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false).OfType<EnumMemberAttribute>().FirstOrDefault();
                string label = enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value)
                    ? enumName
                    : enumMemberAttribute.Value;
                model.Enum.Add(new OpenApiString(label));
            }
        }
        if (context.Type.IsNullableEnum(out var enumType))
        {
            model.Enum.Clear();
            foreach (string enumName in EnumSchemaFilterExtensions.EnumGetNamesEx(enumType))
            {
                System.Reflection.MemberInfo memberInfo = enumType.GetMember(enumName).FirstOrDefault(m => m.DeclaringType == context.Type);
                EnumMemberAttribute enumMemberAttribute = memberInfo == null ? null : memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false).OfType<EnumMemberAttribute>().FirstOrDefault();
                string label = enumMemberAttribute == null || string.IsNullOrWhiteSpace(enumMemberAttribute.Value)
                    ? enumName
                    : enumMemberAttribute.Value;
                model.Enum.Add(new OpenApiString(label));
            }
        }
    }
}

public static class EnumSchemaFilterExtensions
{
    public static string[] EnumGetNamesEx(Type enumType)
    {
        
        var listOfValues =  Enum.GetNames(enumType);
        for (int i = 0; i < listOfValues.Length ; i++)
        {
            var attr = enumType.GetField(listOfValues[i]).GetCustomAttributes(false).OfType<JsonPropertyNameAttribute>().SingleOrDefault();
            if (attr != null)
            {
                listOfValues[i] = attr.Name;    
            }
        }
        return listOfValues.ToArray();  
    }

    public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value);
        return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
    }

    public static bool IsNullableEnum(this Type t, out Type enumType)
    {
        enumType = null;
        var ret = t.IsGenericType &&
               t.GetGenericTypeDefinition() == typeof(Nullable<>) &&
               t.GetGenericArguments()[0].IsEnum;
        if (ret)
        {
            enumType = t.GetGenericArguments()[0];
        }
        return ret;
    }
}