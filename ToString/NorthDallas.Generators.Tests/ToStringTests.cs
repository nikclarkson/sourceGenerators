using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using FluentAssertions;
using Xunit;
using System.Data;

namespace NorthDallas.Generators.Tests
{
    public class ToStringTests
    {
        [Fact]
        public void Should_Generate()
        {
            var testInput = @"using NorthDallas.Generators;

namespace NorthDallas.App.Models
{
    [ToString]
    public partial class UserGroup
    {
        public string Name { get; set; } = ""North Dallas Developers"";

        public string StateMeeting { get; set; } = ""First Wednesdays 6pm"";

        public string MeetingLocation { get; set; } = ""Improving"";

    }
}";

            var output = RunGenerator(testInput);

            output.Should().Be("namespace NorthDallas.App.Models\r\n{\r\n\t partial class UserGroup\r\n\t {\r\n\t\t public override string ToString()\r\n\t\t {\r\n\t\t\t return $\"NorthDallas.App.Models.UserGroup=>[Name]:{Name} [StateMeeting]:{StateMeeting} [MeetingLocation]:{MeetingLocation} \";\r\n\t\t }\r\n\t}\r\n}\r\n");
        }

        private static string? RunGenerator(string sourceCode) 
        {
            
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                                      .Where(assembly => !assembly.IsDynamic)
                                      .Select(assembly => MetadataReference
                                                          .CreateFromFile(assembly.Location))
                                      .Cast<MetadataReference>();

            var compilation = CSharpCompilation.Create("GeneratorTests",
                          new[] { syntaxTree },
                          references,
                          new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            CSharpGeneratorDriver.Create(new ToStringGenerator())
                                 .RunGeneratorsAndUpdateCompilation(compilation,
                                                                    out var outputCompilation,
                                                                    out var diagnostics);

            diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
                       .Should().BeEmpty();

            return outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString();
        }
    }
}
   
