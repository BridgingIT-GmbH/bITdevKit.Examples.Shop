namespace Modules.Catalog.Test.Unit;

using System.Reflection;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Application.Messaging;
using BridgingIT.DevKit.Application.Queries;
using BridgingIT.DevKit.Domain.Repositories;
using Mono.Cecil;
using NetArchTest.Rules;

[UnitTest("Architecture")]
[Module("Catalog")]
public class ArchitectureFitnessTests : TestsBase
{
    private const string BaseNamespace = "Modules";
    private static readonly string ModuleNamespace = $"{BaseNamespace}.{ModuleName.From<ArchitectureFitnessTests>()}";
    private static readonly string ApplicationNamespace = $"{ModuleNamespace}.Application";
    private static readonly string DomainNamespace = $"{ModuleNamespace}.Domain";
    private static readonly string InfrastructureNamespace = $"{ModuleNamespace}.Infrastructure";
    private static readonly string PresentationNamespace = $"{ModuleNamespace}.Presentation";

    public ArchitectureFitnessTests(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public void Fitness_General_Interface_Name_Should_Start_With_An_I()
    {
        var result = Types.InNamespace(ModuleNamespace)
            .That().AreInterfaces()
            .Should().HaveNameStartingWith("I")
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Messaging_Message_Should_Exist_In_Application_Layer_Only()
    {
        var result = Types.InNamespace(ModuleNamespace)
            .That().Inherit(typeof(MessageBase)).Or().ImplementInterface(typeof(IMessage))
            .Should().ResideInNamespace($"{ModuleNamespace}.Application")
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Messaging_Message_Should_Be_Named_Correctly()
    {
        var result = Types.InNamespace(ModuleNamespace)
           .That().Inherit(typeof(MessageBase)).Or().ImplementInterface(typeof(IMessage))
           .Should().HaveNameEndingWith("Message")
           .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Commands_Command_Should_Exist_In_Application_Layer_Only()
    {
        var result = Types.InNamespace(ModuleNamespace)
            .That().Inherit(typeof(CommandRequestBase)).Or().Inherit(typeof(CommandRequestBase<>)).Or().ImplementInterface(typeof(ICommandRequest)).Or().ImplementInterface(typeof(ICommandRequest<>))
            .Should().ResideInNamespace($"{ModuleNamespace}.Application")
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Commands_Command_Should_Be_Named_Correctly()
    {
        var result = Types.InNamespace(ModuleNamespace)
           .That().Inherit(typeof(CommandRequestBase)).Or().Inherit(typeof(CommandRequestBase<>)).Or().ImplementInterface(typeof(ICommandRequest)).Or().ImplementInterface(typeof(ICommandRequest<>))
           .Should().HaveNameEndingWith("Command")
           .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Queries_Query_Should_Exist_In_Application_Layer_Only()
    {
        var result = Types.InNamespace(ModuleNamespace)
            .That().Inherit(typeof(QueryRequestBase<>)).Or().ImplementInterface(typeof(IQueryRequest<>))
            .Should().ResideInNamespace($"{ModuleNamespace}.Application")
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Queries_Query_Should_Be_Named_Correctly()
    {
        var result = Types.InNamespace(ModuleNamespace)
           .That().Inherit(typeof(QueryRequestBase<>)).Or().ImplementInterface(typeof(IQueryRequest<>))
           .Should().HaveNameEndingWith("Query")
           .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Application_Layer_Should_Not_Depend_On_Infrastructure_Layer()
    {
        var sourceSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(ApplicationNamespace);

        var invalidSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(InfrastructureNamespace);

        var result = sourceSlice
            .ShouldNot().HaveDependencyOnAny(
                invalidSlice.GetTypes().Select(t => t.FullName).Distinct().ToArray())
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Application_Layer_Should_Not_Depend_On_Presentation_Layer()
    {
        var sourceSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(ApplicationNamespace);

        var invalidSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(PresentationNamespace);

        var result = sourceSlice
            .ShouldNot().HaveDependencyOnAny(
                invalidSlice.GetTypes().Select(t => t.FullName).Distinct().ToArray())
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Domain_Layer_Should_Not_Depend_On_Application_Layer()
    {
        var sourceSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(DomainNamespace);

        var invalidSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(ApplicationNamespace);

        var result = sourceSlice
            .ShouldNot().HaveDependencyOnAny(
                invalidSlice.GetTypes().Select(t => t.FullName).Distinct().ToArray())
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Domain_Layer_Should_Not_Depend_On_Infrastructure_Layer()
    {
        var sourceSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(DomainNamespace);

        var invalidSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(InfrastructureNamespace);

        var result = sourceSlice
            .ShouldNot().HaveDependencyOnAny(
                invalidSlice.GetTypes().Select(t => t.FullName).Distinct().ToArray())
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Domain_Layer_Should_Not_Depend_On_Presentation_Layer()
    {
        var sourceSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(DomainNamespace);

        var invalidSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(PresentationNamespace);

        var result = sourceSlice
            .ShouldNot().HaveDependencyOnAny(
                invalidSlice.GetTypes().Select(t => t.FullName).Distinct().ToArray())
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Infrastructure_Layer_Should_Not_Depend_On_Presentation_Layer()
    {
        var sourceSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(InfrastructureNamespace);

        var invalidSlice = Types.InNamespace(ModuleNamespace)
            .That().ResideInNamespaceStartingWith(PresentationNamespace);

        var result = sourceSlice
            .ShouldNot().HaveDependencyOnAny(
                invalidSlice.GetTypes().Select(t => t.FullName).Distinct().ToArray())
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Domain_Repository_Interface_Should_Exist_In_Domain_Layer_Only()
    {
        var result = Types.InNamespace(ModuleNamespace)
            .That().ImplementInterface(typeof(IRepository)).And().AreInterfaces()
            .Should().ResideInNamespace($"{ModuleNamespace}.Domain")
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Fitness_Domain_Repository_Implementation_Should_Exist_In_Infrastructure_Layer_Only()
    {
        var result = Types.InNamespace(ModuleNamespace)
            .That().ImplementInterface(typeof(IRepository)).And().AreClasses()
            .Should().ResideInNamespace($"{ModuleNamespace}.Infrastructure")
            .GetResult();

        result.FailingTypes.ForEach(t => this.Output?.WriteLine($"failing type: {t.FullName}"));
        result.IsSuccessful.ShouldBeTrue();
    }

    // TODO: move to Bit.Baukasten.Common.Utilities.Xunit
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