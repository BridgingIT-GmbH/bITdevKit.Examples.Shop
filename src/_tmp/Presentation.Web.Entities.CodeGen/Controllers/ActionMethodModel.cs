namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

public class ActionMethodModel
{
    public string Name { get; init; }

    public string Module { get; init; }

    public string ActionName { get; init; }

    public string RequestName { get; init; }

    public string EntityName { get; init; }

    public string Template { get; init; }

    public bool Authorize { get; init; }

    public string Policy { get; init; }

    public string HttpMethod { get; init; }

    public List<ParameterModel> Parameters { get; init; } = new();

    public static ActionMethodModel Create(ActionCandidate candidate)
    {
        return new ActionMethodModel()
        {
            Name = candidate.BaseInterface.Name.Substring(1),
            ActionName = candidate.RequestType.Name,
            RequestName = $"{candidate.RequestTypeNamespace}.{candidate.RequestType.Name}",
            EntityName = $"{candidate.EntityNamespace}.{candidate.EntityType.Name}",
            Module = candidate.Attribute.NamedArguments.FirstOrDefault(a => a.Key == "Module").Value.Value as string ?? string.Empty,
            Template = candidate.Attribute.NamedArguments.FirstOrDefault(a => a.Key == "Template").Value.Value as string ?? string.Empty, // route
            Authorize = candidate.Attribute.NamedArguments.FirstOrDefault(a => a.Key == "Authorize").Value.Value as bool? ?? false,
            Policy = candidate.Attribute.NamedArguments.FirstOrDefault(a => a.Key == "Policy").Value.Value as string ?? string.Empty, // authorize
            Parameters = CreateParameters(candidate, candidate.RequestType).ToList()
        };
    }

    private static IEnumerable<ParameterModel> CreateParameters(ActionCandidate candidate, INamedTypeSymbol typeSymbol)
    {
        var attributeParametersValue = candidate.Attribute.NamedArguments.FirstOrDefault(a => a.Key == "Parameters").Value;

        if (!attributeParametersValue.IsNull)
        {
            var attributeParameters = attributeParametersValue.Values;
            if (attributeParameters != default)
            {
                var typeProperties = typeSymbol.GetProperties();

                foreach (var parameter in attributeParameters)
                {
                    var parameterName = parameter.Value as string;
                    if (typeProperties.ContainsKey(parameterName))
                    {
                        yield return new ParameterModel(parameterName, typeProperties[parameterName].Name);
                    }
                }
            }
        }
    }
}