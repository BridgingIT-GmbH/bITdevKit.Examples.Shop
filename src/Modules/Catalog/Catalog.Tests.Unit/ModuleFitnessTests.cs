namespace Modules.Catalog.Test.Unit;

using System.Reflection;
using Mono.Cecil;
using NetArchTest.Rules;

[UnitTest("Architecture")]
[Module("Catalog")]
public class ModuleFitnessTests : TestsBase
{
    private const string BaseNamespace = "Modules";
    private static readonly string ModuleNamespace = $"{BaseNamespace}.{ModuleName.From<ArchitectureFitnessTests>()}";

    public ModuleFitnessTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public void Fitness_Module_Should_Not_Reference_Other_Modules()
    {
        var result = Types.InCurrentDomain()
            .That().ResideInNamespaceStartingWith(BaseNamespace)
            .Should().MeetCustomRule(
                new ExpectedNamespaceRule(BaseNamespace, ModuleNamespace))
            .GetResult();

        result.FailingTypes?.DistinctBy(t => t.Assembly.FullName).ForEach(t => this.Output?.WriteLine($"failing assembly: {t.Assembly.GetName().Name}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    // TODO: move to Bit.Baukasten.Common.Utilities.Xunit\Fitness
    private class ExpectedNamespaceRule : ICustomRule
    {
        // https://github.com/BenMorris/NetArchTest/pull/80
        private readonly string baseNamespace;
        private readonly string expectedNamespace;

        public ExpectedNamespaceRule(string baseNamespace, string expectedNamespace)
        {
            this.baseNamespace = baseNamespace;
            this.expectedNamespace = expectedNamespace;
        }

        public bool MeetsRule(TypeDefinition type)
        {
            return !type.Module.GetTypeReferences()
                .Any(t => !t.Namespace.StartsWith("Bit.Baukasten") && t.Namespace.StartsWith($"{this.baseNamespace}.") && !t.Namespace.StartsWith(this.expectedNamespace));
        }
    }
}