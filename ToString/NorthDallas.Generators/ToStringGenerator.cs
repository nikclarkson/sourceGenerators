using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NorthDallas.Generators
{
    [Generator]
    public class ToStringGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {

            context.RegisterPostInitializationOutput(postInitContext => GenerateMarkerAttribute(postInitContext));

            var classModels = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (syntaxNode, _) => IsTarget(syntaxNode),
                transform: static (generatorSyntaxContext, _) => GetClassModel(generatorSyntaxContext))
                .Where(static (classModel) => classModel is not null);

            context.RegisterSourceOutput(classModels,
                static (ctx, source) => GenerateClass(ctx, source));
        }

        private static void GenerateMarkerAttribute(IncrementalGeneratorPostInitializationContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("namespace NorthDallas.Generators");
            sb.AppendLine("{");
            sb.AppendLine("\t class ToStringAttribute : System.Attribute { }");
            sb.AppendLine("}");

            context.AddSource("ToStringAttribute.g.cs", sb.ToString());

        }

        private static bool IsTarget(SyntaxNode node)
        {
            return node is 
                ClassDeclarationSyntax classDeclarationSyntax && 
                classDeclarationSyntax.AttributeLists.Any();
        }

        private static StringClassModel? GetClassModel(
            GeneratorSyntaxContext context)
        {
            var classSymbol = context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node);

            var markerAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName("NorthDallas.Generators.ToStringAttribute");

            if (classSymbol is null || markerAttributeSymbol is null)
                return null;

            if(classSymbol.GetAttributes().Any(attribute => markerAttributeSymbol.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default)))
            {
                var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
                var className = classSymbol.Name;
                var propertyNames = new List<string>();


                var memberNames = classSymbol.GetMembers().Where(
                    memberSymbol => memberSymbol.Kind == SymbolKind.Property && 
                                    memberSymbol.DeclaredAccessibility == Accessibility.Public)
                                    .Select(member => member.Name);

                propertyNames.AddRange(memberNames);

                return new StringClassModel(namespaceName, className, propertyNames);
            }
            

            return null;
        }

        private static void GenerateClass(SourceProductionContext context,
            StringClassModel? classModel)
        {
            var namespaceName = classModel!.NamespaceName;
            var className = classModel.ClassName;
            var fileName = $"{namespaceName}.{className}.g.cs";

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"namespace {namespaceName}");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"\t partial class {className}");
            stringBuilder.AppendLine("\t {");
            stringBuilder.AppendLine($"\t\t public override string ToString()");
            stringBuilder.AppendLine("\t\t {");

            stringBuilder.Append($"\t\t\t return $\"{namespaceName}.{className}=>");

            foreach (var propertyName in classModel.PropertyNames)
            {
                stringBuilder.Append($"[{propertyName}]:{{{propertyName}}} ");
            }

            stringBuilder.Append("\";\r\n");

            stringBuilder.AppendLine("\t\t }");
            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");
          
            context.AddSource(fileName, stringBuilder.ToString());
        }
    }
}
