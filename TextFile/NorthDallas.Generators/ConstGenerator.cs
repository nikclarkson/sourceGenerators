using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace NorthDallas.Generators
{
    [Generator]
    public class ConstGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {

            var textFiles = initContext.AdditionalTextsProvider.Where(file => file.Path.EndsWith(".txt"));

            var fileContents = textFiles.Select((text, _) => (name: Path.GetFileNameWithoutExtension(text.Path), contents: text.GetText()!.ToString()));

            initContext.RegisterSourceOutput(fileContents,
                (context, fileContents) => context.AddSource($"{fileContents.name}.g.cs", Generate(fileContents.name, fileContents.contents))); 
            
        }

        private string Generate(string name, string constants)
        {
            var lines = constants.Split('\n');

            var sb = new StringBuilder();

            sb.AppendLine("namespace CodeGen {");
            sb.Append($"public static partial class {name} {{");
            sb.Append("\r\n");

            foreach (var line in lines)
            {
                sb.AppendLine($"\t public const string {line} = \"{line}\";");
            }

            sb.Append("\r\n");
            sb.Append("}}");

            return sb.ToString();
        }
    }
}
