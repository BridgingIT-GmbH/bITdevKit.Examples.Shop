namespace BridgingIT.DevKit.Presentation.Web.Entities.CodeGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

public partial record ControllerModel
{
    internal static ControllersModelBuilder Builder(
        GeneratorExecutionContext context)
        => new(context);

    private static ControllerModel Build(
        string name,
        IEnumerable<ActionCandidate> actions,
        Compilation compilation)
    {
        return new ControllerModel()
        {
            Name = name,
            Namespace = $"{compilation.AssemblyName}.Web.Controllers",
            Methods = actions.Select(a => ActionMethodModel.Create(a))
        };
    }

    public class ControllersModelBuilder
    {
        private readonly GeneratorExecutionContext context;
        private readonly Compilation compilation;
        private readonly Dictionary<string, List<ActionCandidate>> controllers = new(StringComparer.OrdinalIgnoreCase);

        public ControllersModelBuilder(GeneratorExecutionContext context)
        {
            this.context = context;
            this.compilation = context.Compilation;
        }

        public void AddCandidate(ActionCandidate candidate)
        {
            var controllerName = candidate.Attribute.NamedArguments.FirstOrDefault(a => a.Key == "Controller").Value.Value as string ?? candidate.EntityType.Name; //.CheckControllerName();

            if (!this.controllers.ContainsKey(controllerName))
            {
                this.controllers.Add(controllerName, new List<ActionCandidate>());
            }

            this.controllers[controllerName].Add(candidate);
        }

        public IEnumerable<ControllerModel> Build()
            => this.controllers.Select(p => ControllerModel.Build(p.Key, p.Value, this.compilation));
    }
}