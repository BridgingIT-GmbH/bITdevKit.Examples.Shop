namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;

public enum TemplateType
{
#pragma warning disable SA1602 // Enumeration items should be documented
    Controller,
    ControllerUsings,
    ControllerAttributes,
    ControllerMethods,
    //MethodBody,
    //MethodAttributes,
    ControllerEntityCreateMethodBody,
    ControllerEntityUpdateMethodBody,
    ControllerEntityDeleteMethodBody,
    ControllerEntityFindAllMethodBody,
    ControllerEntityFindOneMethodBody
#pragma warning restore SA1602 // Enumeration items should be documented
}