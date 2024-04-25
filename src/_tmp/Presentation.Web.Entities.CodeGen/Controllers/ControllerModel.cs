namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;

public partial record ControllerModel
{
    public string Namespace { get; init; }

    public string Name { get; init; }

    public IEnumerable<ActionMethodModel> Methods { get; init; }
}